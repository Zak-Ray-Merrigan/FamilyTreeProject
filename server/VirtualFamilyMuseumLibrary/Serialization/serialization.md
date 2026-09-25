## Serialization
# Table Of Contents
1. Overview
2. Why JSON Is the Single Source of Truth
3. The Bridge Abstraction
4. Models
5. Bridge Serializer
6. Serialization Extensions
7. Implementing IBridge on a Domain Type
8. Mapping Between C# .NET 9, JSON, Cosmos DB, Neo4j, and React
9. Serving the React Client
10. Known Limitations
11. Tests

# Overview
The Serialization aspect is the single source of truth for JSON communication between the four places a value lives. **C# .NET 9 is the central hub:** the domain model in `VirtualFamilyMuseumLibrary` sits in the middle, and the other three connect only to it:

```
 Cosmos DB (documents)        Neo4j (vertices)
            \                      /
             ⇅                    ⇅
          C# .NET 9 (hub: VirtualFamilyMuseumLibrary)
                        ⇅
       React (client, over the ASP.NET Core Web API)
```

The edges never talk to each other directly: React never reads Cosmos DB, and Neo4j never receives a Cosmos DB document. Each domain type describes itself once, in the hub, as a JSON value, and each edge's adapter translates to and from that one value.

The aspect is small:
- `IBridge` / `Bridge`: the abstraction a domain type implements (or is wrapped in) to expose its JSON value.
- `BridgeValue`: an immutable, strongly typed model of a single JSON value.
- `Number`: the numeric payload of a `BridgeValue`.
- `BridgeSerializer`: the `System.Text.Json` converter that turns an `IBridge` into JSON text and back.
- `SerializationExtensions`: the shared `JsonSerializerOptions` and the typed `Serialize*`/`Deserialize*` helpers for converting between C# primitives and bridges.

# Why JSON Is the Single Source of Truth
The places a value travels to disagree about what data looks like:
- **C# .NET 9** has a rich static type system: `int` vs. `long` vs. `double`, `Guid`, structs such as `FamilyDate`, and nullable value types.
- **Cosmos DB (NoSQL API)** stores JSON documents natively. Objects nest, arrays can hold mixed kinds, and every number is an IEEE 754 double.
- **Neo4j** stores properties on vertices (nodes) and edges (relationships). A property can only be a primitive (string, 64-bit integer, 64-bit float, boolean, temporal, spatial) or a homogeneous list of primitives. Maps do not nest, lists cannot mix kinds, and a `null` property is not stored at all.
- **React (JavaScript)** gets values from `response.json()` / `JSON.parse`. It has one number type (a double that is exact only up to `Number.MAX_SAFE_INTEGER`, 2^53 − 1), no `Guid` or date-only type, and two kinds of "missing": `null` and `undefined`.

Writing a separate mapping from every domain type to every destination would multiply as types and destinations are added, and the mappings would drift apart. Instead, each domain type maps to exactly one representation (a JSON value, modeled by `BridgeValue`) and each destination maps from that representation. Adding a domain type means writing one mapping. Adding a store, or another client, means writing one adapter. When two destinations disagree about what a value means, the JSON value decides.

JSON was chosen as the pivot representation because:
- It is Cosmos DB's own storage format and the React client's own wire format, so neither of those directions needs translation.
- Its six kinds (null, string, number, boolean, object, array) cover every value a family tree record needs.
- `System.Text.Json` already provides a streaming reader and writer, so the converter only handles the type mapping.
- Any serialized value can be read as text, which makes debugging and test assertions easier.

# The Bridge Abstraction
The name refers to the Bridge design pattern: the *abstraction* (what a domain value is) is kept separate from the *implementation* (how a particular store persists it), so each side can change without the other.

- `IBridge`: a single read-only property, `BridgeValue Value`. Anything that implements it can be serialized by `BridgeSerializer`. Domain types implement it directly. `FamilyDate` is the first one (see Implementing IBridge on a Domain Type).
- `Bridge`: the default, general-purpose implementation. It is a thin immutable wrapper around a `BridgeValue`, taken through its primary constructor. It is what `BridgeSerializer.Read` returns and what every `Serialize*` extension method produces. `Bridge` has value semantics:
  - `Equals`/`GetHashCode` delegate to the wrapped `BridgeValue`, so two bridges holding equal JSON values are equal.
  - `==`/`!=` are null-safe: two `null` references are equal; a `null` and a non-`null` are not.
  - `ToString()` delegates to `BridgeValue.ToString()` (indented JSON; see Known Limitations for the kinds this doesn't yet work for).

# Models
## BridgeValue
A `readonly struct` holding exactly one JSON value, stored privately as an `object?` whose runtime type determines its kind:

| JSON kind | Stored as | Created by | Read by |
|---|---|---|---|
| null | `null` | `new BridgeValue()` | `IsNull` |
| string | `string` | implicit conversion from `string` | explicit `(string)` cast, `TryAsString` |
| number | `Number` | implicit conversion from `Number` | explicit `(Number)` cast, `TryAsNumber` |
| boolean | `bool` | implicit conversion from `bool` | explicit `(bool)` cast, `TryAsBool` |
| object | `Dictionary<string,BridgeValue>` | `new BridgeValue(IDictionary<string,BridgeValue>)` | `AsObject`, `TryAsObject` |
| array | `List<BridgeValue>` | `new BridgeValue(IList<BridgeValue>)` | `AsArray`, `TryAsArray` |

Design points:
- **Closed set of kinds.** The string, number and boolean constructors are private, so the only ways in are the typed conversions and the two public collection constructors. A `BridgeValue` can't hold an arbitrary CLR object.
- **Immutability through defensive copies.** The object and array constructors copy their input, and `AsObject`, `AsArray`, `TryAsObject` and `TryAsArray` each return a fresh copy. Mutating a collection you passed in or got back never changes the `BridgeValue`. The copy is shallow, but nested values are themselves immutable `BridgeValue`s, so the whole tree is effectively immutable.
- **Two ways to read.** The explicit casts and `AsObject`/`AsArray` throw `InvalidCastException` when the value is null ("The value doesn't exist.") or a different kind ("The value isn't of type …"). The `TryAs*` methods never throw. They return `false` and a `null` out-parameter instead. Use the casts when the shape is already known (a schema you control) and `TryAs*` when inspecting an unknown shape (e.g. `BridgeSerializer.WriteValue`).
- **Structural equality.** `Equals` compares kind first, then content: strings by ordinal equality, numbers by `Number` equality, booleans by value, objects by same key set with equal values (order-insensitive), and arrays element by element in order. Values of different kinds are never equal, so the string `"1"` does not equal the number `1`.
- **Hash codes consistent with equality.** Object hashes are order-insensitive: each `(key, value)` pair is hashed, the pair hashes are sorted, and they are combined as a weighted sum over successive primes. Array hashes use the same prime-weighted sum, but in element order, so `[1,2]` and `[2,1]` hash differently.

## Number
A `struct` wrapping a single `double`. JSON has one number kind and so does `Number`. Integer vs. floating-point is not stored; it is decided when the value is read:
- `TryGetInt` / `TryGetLong` return `true` only if the value is whole and within range.
- The explicit `(int)` / `(long)` casts do the same checks but throw `InvalidCastException` ("The value must be whole." or the range message) instead of returning `false`.
- `(double)` and `TryGetDouble` always succeed.
- Implicit conversions from `double` and `long` (and from `int`, through `long`) mean numeric literals can be passed wherever a `Number` is expected.
- Equality and ordering (`IComparable<Number>`, `==`, `<`, `>=`, …) compare the underlying doubles, so `(Number)1L == (Number)1.0`.

# Bridge Serializer
`BridgeSerializer` is a `JsonConverter<IBridge>`. It is the only place JSON text is produced from or turned into a `BridgeValue`.

**Reading** (`Read`) walks the `Utf8JsonReader` token stream recursively and always returns a `Bridge`:

| Token | Becomes |
|---|---|
| `Null` | `new BridgeValue()` |
| `String` | string `BridgeValue` |
| `Number` | `Number` via `reader.GetDouble()` |
| `True` / `False` | boolean `BridgeValue` |
| `StartObject` | object `BridgeValue`, reading `PropertyName`/value pairs until `EndObject`. A non-property token where a property name is expected throws `JsonException("No Property Name found")`. |
| `StartArray` | array `BridgeValue`, reading values until `EndArray` |
| anything else | `JsonException("<token> isn't supported.")` |

`HandleNull` is `true`, so a JSON `null` in an `IBridge` slot becomes a `Bridge` holding a null `BridgeValue` rather than a `null` reference. A deserialized `IBridge` is never `null`.

**Writing** (`Write`) does the reverse, choosing the kind with the `TryAs*` methods. Numbers are written in the narrowest form that represents them exactly: `int` if the value is whole and fits, otherwise `long` if it fits, otherwise `double`. `3.0` is therefore written as `3`, not `3.0`. Keeping whole numbers integral in the text matters for Neo4j, which stores integers and floats as separate types (see Mapping Between C#, JSON, Cosmos DB, and Neo4j).

Object keys are written exactly as stored. The converter writes them itself, so the `CamelCase` naming policy in `GetOptions` never renames keys inside a `BridgeValue`. It only renames properties of ordinary C# classes that are serialized with the same options.

# Serialization Extensions
## GetOptions(bool writeIndented)
Returns the `JsonSerializerOptions` every part of this project should use. It registers `BridgeSerializer`, sets `PropertyNamingPolicy = JsonNamingPolicy.CamelCase`, and sets `WriteIndented` from the argument. Always serialize with these options rather than building `JsonSerializerOptions` by hand, so the converter and naming policy stay the same everywhere. The camelCase policy is chosen to match JavaScript naming, so the React client reads `birthDate`, not `BirthDate`.

## Serialize* / Deserialize* pairs
Each method is an extension on `IBridge`. `Deserialize*` reads the receiver's value as a C# type. `Serialize*` ignores the receiver and returns a **new** `Bridge` holding the input, so any existing bridge can serve as the receiver:

| C# type | Serialize | Deserialize | JSON form | Failure |
|---|---|---|---|---|
| `null` | `SerializeNull()` | `DeserializeNull()` → `null` | `null` | `InvalidCastException("The value does exist.")` if not null |
| `string` | `SerializeString` | `DeserializeString` | string | `InvalidCastException` if not a string |
| `int` | `SerializeInt` | `DeserializeInt` | number | `InvalidCastException` if not a whole number in `int` range |
| `long` | `SerializeLong` | `DeserializeLong` | number | `InvalidCastException` if not a whole number in `long` range |
| `double` | `SerializeDouble` | `DeserializeDouble` | number | `InvalidCastException` if not a number |
| object | `SerializeObject` | `DeserializeObject` | object | `InvalidCastException` if not an object |
| array | `SerializeArray` | `DeserializeArray` | array | `InvalidCastException` if not an array |
| `Guid` | `SerializeGuid` | `DeserializeGuid` | string (`Guid.ToString()`, "D" format) | `InvalidCastException("Not a GUID.")` |
| `FamilyDate` | *(none; `FamilyDate` is itself an `IBridge`)* | `DeserializeDate` | string (e.g. `"5 Mar 1940"`) | `InvalidCastException("This text is not a date")` |

Not every type has both directions:
- **`FamilyDate`** has no `SerializeDate`. It implements `IBridge` itself (its `Value` is its `ToString()`), so a `FamilyDate` already is its serialized form. `DeserializeDate` parses the string back through `FamilyDate.GetDate`.
- **`bool`** has no pair yet. Use `new Bridge(true)` and `(bool)bridge.Value` directly.

# Implementing IBridge on a Domain Type
A domain type becomes serializable by implementing `IBridge` and returning its canonical JSON value from `Value`. `FamilyDate` (`Models/FamilyDate.cs`) is the reference example:

```csharp
public readonly partial struct FamilyDate(string year, Month? month = null, int? day = null) : IBridge, ...
{
    public BridgeValue Value
    {
        get
        {
            return ToString();   // "1940", "Mar 1940", "5 Mar 1940", "1940–1943"
        }
    }
}
```

Guidelines for new types:
- **Choose the simplest JSON kind that round-trips.** `FamilyDate` uses a string rather than an object `{ year, month, day }`. A string keeps partial dates and year ranges in one field and stores as a single primitive property in Neo4j without flattening.
- **Pair `Value` with a parser.** Whatever `Value` produces must be readable back by a `Deserialize*` extension (for `FamilyDate`, `DeserializeDate` → `FamilyDate.GetDate`). The test for that type should check `Deserialize(Serialize(x)) == x`.
- **Keep business rules out.** `Value` describes the data. It doesn't validate it. Validation belongs outside the model and serialization layers.

# Mapping Between C# .NET 9, JSON, Cosmos DB, Neo4j, and React
> **Status:** the Cosmos DB and Neo4j adapters, the Web API, and the React client source are not built yet. This section records the contract they must follow. It will be updated as each one is implemented.

| `BridgeValue` kind | C# .NET 9 (via extensions) | JSON | Cosmos DB document | Neo4j vertex property | React (JavaScript) |
|---|---|---|---|---|---|
| null | `null` | `null` | `null` field (kept) | property **omitted**. Neo4j does not store null, so a missing property reads back as null. | `null`, or the key may be absent (`undefined`); see Serving the React Client |
| string | `string`, `Guid`, `FamilyDate` | string | string | `String` | `string` |
| number (whole) | `int`, `long` | integer literal | number (double) | `Integer` (64-bit) | `number` (exact up to 2^53 − 1) |
| number (fractional) | `double` | decimal literal | number (double) | `Float` (64-bit) | `number` |
| boolean | `bool` | `true`/`false` | boolean | `Boolean` | `boolean` |
| array (all elements one primitive kind) | `IList<BridgeValue>` | array | array | homogeneous `List` | `Array` |
| array (mixed kinds or nested) | `IList<BridgeValue>` | array | array | not representable as a property; see below | `Array` |
| object | `IDictionary<string,BridgeValue>` | object | nested object | not representable as a property; see below | plain object |

Consequences for the adapters:
- **Cosmos DB is a direct mapping.** `BridgeSerializer` output is already a valid document body. The adapter's only extra work is Cosmos DB's own rules: every document needs a string `id` (use `SerializeGuid`), a partition key property, and must not write the system properties (`_rid`, `_self`, `_etag`, `_attachments`, `_ts`).
- **Neo4j needs one level of flattening.** A vertex is a single-level map of primitive properties, so the top-level `BridgeValue` for a vertex must be an object whose values are primitives or homogeneous primitive arrays. Nested objects and mixed arrays must either become separate vertices joined by edges (usually right for graph data such as family relationships) or be stored as a JSON string property with `ToString()` / `JsonSerializer.Deserialize<IBridge>`. The Neo4j adapter should make this choice in one place, not leave it to each call site.
- **Keep the integer/float distinction.** Because `BridgeSerializer` writes whole numbers as integer literals, the Neo4j adapter can pass `TryGetLong` values as `Integer` and everything else as `Float`, so a value written as `3` reads back as `3` and not `3.0`.

# Serving the React Client
> **Status:** neither the ASP.NET Core Web API (`ExecutionTypes.API`) nor the React client source exists yet (`client/` only holds an old `build/` output). These are the rules both must follow.

**The API must use `GetOptions`.** ASP.NET Core in .NET 9 serializes responses and binds request bodies with its own `JsonSerializerOptions`, not with `GetOptions()`. When the API is built, copy the same settings into the framework's options so the React client sees exactly what the stores see:

```csharp
// Minimal APIs
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new BridgeSerializer());
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Controllers
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new BridgeSerializer());
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
```

These settings belong in a single shared helper, so the API cannot drift from `GetOptions`. `SerializationExtensions` already imports `Microsoft.AspNetCore.Http.Json` (currently unused), so it's the natural home for that helper.

**What the React client must assume:**
- **Identifiers are strings.** JavaScript numbers are exact only up to 2^53 − 1, the same ceiling as `BridgeSerializer.Read` (see Known Limitations). Every identifier crosses the wire as a `Guid` string (`SerializeGuid`) and is compared as a string. Never convert one with `Number(...)`.
- **`FamilyDate` is display text, not a `Date`.** It arrives as `"1940"`, `"Mar 1940"`, `"5 Mar 1940"`, or a range with an en dash (`"1940–1943"`). Show it as-is. Don't parse it with `new Date(...)`: a partial date would become an invented day, and a range would become `Invalid Date`. When sending a date back, send the same text format; `DeserializeDate` rejects anything `FamilyDate.GetDate` can't parse.
- **Treat a missing key the same as `null`.** A value that came from Neo4j may be absent rather than `null` (Neo4j doesn't store nulls). Read optional fields with `value ?? fallback`, not `value === null`. When sending, remember that `JSON.stringify` drops keys whose value is `undefined`. Send `null` explicitly when a field must be cleared.
- **Integers and decimals look the same.** `BridgeSerializer` writes `3` rather than `3.0`, and JavaScript can't tell them apart. Where the difference matters (e.g. a generation number must be whole), the server's `DeserializeInt`/`DeserializeLong` checks it on the way back in. The client should not rely on the JSON text to say so.
- **Key casing depends on where the key comes from.** Properties of C# classes arrive in camelCase because of the naming policy. Keys inside a `BridgeValue` object arrive exactly as they were stored (see Bridge Serializer). Build those objects with camelCase keys, so the client never sees mixed casing.

# Known Limitations
These were confirmed by running the code as of 2026-09-25 and are not covered by the current tests:
- **`BridgeValue.ToString()` only works for null, string and boolean values.** It calls `JsonSerializer.Serialize(value, …)` on the raw stored object rather than through `BridgeSerializer`. A number prints as `{}`, and an object or array throws `InvalidCastException` (reflection hits the `AsObject` getter of the nested `BridgeValue`s). `Bridge.ToString()` delegates to it and fails the same way. Serializing `new Bridge(this)` as `IBridge` would route every kind through the converter.
- **`Number.ToString()` can throw `OverflowException`.** When the fractional digits of a `double` don't fit in an `int` (e.g. `Math.PI`), `Convert.ToInt32(decimalPart)` overflows. `double.ToString()` already drops a trailing `.0`, so that branch can't change the output for any value that does not throw.
- **The converter only runs for values whose declared type is exactly `IBridge`.** `JsonConverter<IBridge>` only matches `typeof(IBridge)`. Serializing a `FamilyDate` directly, or a C# class with a `FamilyDate` property, falls back to reflection and throws. Declare such members as `IBridge` for now. Supporting concrete types properly would need a `JsonConverterFactory` (or a `CanConvert` override) for writing, plus a per-type way to construct the concrete type when reading.
- **Integers above 2^53 lose precision.** `Read` always calls `reader.GetDouble()`, so `9007199254740993` comes back as `9007199254740992`. This matches Cosmos DB and JavaScript, which both use doubles, but is narrower than Neo4j's 64-bit `Integer`. Avoid numeric identifiers this large; use `Guid`s serialized as strings.

# Tests
Every type in this aspect has an NUnit fixture in `VirtualFamilyMuseumLibraryTest/Serialization`, mirroring the source layout:
- `BridgeTest.cs`: construction, value equality, hash codes, null-safe operators, and `ToString` for null/string/bool.
- `BridgeSerializerTest.cs`: reading each token kind, writing each value kind, number-width selection, and round trips.
- `SerializationExtensionsTest.cs`: `GetOptions`, and every `Serialize*`/`Deserialize*` pair (including `DeserializeDate`) with each failure case.
- `Models/BridgeValueTest.cs`: every construction route, the defensive copies, structural equality and hashing, casts and `TryAs*`.
- `Models/NumberTest.cs`: range and whole-number checks, conversions, and comparison operators.

`FamilyDate`'s own `Value`/`ToString()` and `GetDate` parsing are tested with the rest of that type in `VirtualFamilyMuseumLibraryTest/Models/FamilyDateTest.cs`.
