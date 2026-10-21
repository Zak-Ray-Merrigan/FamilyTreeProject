using System.Text;
using System.Text.Json;
using VirtualFamilyMuseumLibrary.Documents.Models;
using VirtualFamilyMuseumLibrary.Models;
using VirtualFamilyMuseumLibrary.Serialization;
using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibraryTest.Serialization
{
    [TestFixture]
    public class DocumentSerializerTest
    {
        private const string PersonId = "6f1c2a7e-3b4d-4e5f-8a9b-0c1d2e3f4a5b";

        private static readonly DocumentSerializer serializer = new();

        private static Dictionary<string,BridgeValue> CreatePersonAttributes()
        {
            return new Dictionary<string,BridgeValue>()
            {
                {"id", PersonId},
                {"birthFamilyName", "Kessler"},
                {"birthName", "Colby Bryan Kessler"},
                {"birthDate", "20 Feb 1888"},
                {"deceasedDate", new BridgeValue()}
            };
        }

        private static MemoryStream ToJsonStream(string json)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(json));
        }

        private static string ReadAll(Stream stream)
        {
            using StreamReader reader = new(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        // =====================================================================
        // ToStream — supported input
        // =====================================================================

        [Test]
        public void ToStream_Person_WritesDocumentWithExactKeysAndValues()
        {
            Person person = new(CreatePersonAttributes());

            using JsonDocument json = JsonDocument.Parse(ReadAll(serializer.ToStream(person)));
            JsonElement root = json.RootElement;

            Assert.That(root.ValueKind, Is.EqualTo(JsonValueKind.Object));
            Assert.That(root.EnumerateObject().Select(p => p.Name), Is.EquivalentTo(new[] { "id", "birthFamilyName", "birthName", "birthDate", "deceasedDate" }));
            Assert.That(root.GetProperty("id").GetString(), Is.EqualTo(PersonId));
            Assert.That(root.GetProperty("birthFamilyName").GetString(), Is.EqualTo("Kessler"));
            Assert.That(root.GetProperty("birthName").GetString(), Is.EqualTo("Colby Bryan Kessler"));
            Assert.That(root.GetProperty("birthDate").GetString(), Is.EqualTo("20 Feb 1888"));
            Assert.That(root.GetProperty("deceasedDate").ValueKind, Is.EqualTo(JsonValueKind.Null));
        }

        [Test]
        public void ToStream_Person_ReturnsStreamRewoundToStart()
        {
            Person person = new(CreatePersonAttributes());

            Stream stream = serializer.ToStream(person);

            Assert.That(stream.Position, Is.EqualTo(0));
            Assert.That(stream.Length, Is.GreaterThan(0));
        }

        [Test]
        public void ToStream_Person_WritesCompactJson()
        {
            Person person = new(CreatePersonAttributes());

            string json = ReadAll(serializer.ToStream(person));

            Assert.That(json, Does.Not.Contain("\n"));
        }

        [Test]
        public void ToStream_BridgeDeclaredAsIBridge_WritesValue()
        {
            IBridge bridge = new Bridge(new BridgeValue(CreatePersonAttributes()));

            using JsonDocument json = JsonDocument.Parse(ReadAll(serializer.ToStream(bridge)));

            Assert.That(json.RootElement.GetProperty("birthName").GetString(), Is.EqualTo("Colby Bryan Kessler"));
        }

        [Test]
        public void ToStream_StringBridgeDeclaredAsObject_WritesJsonString()
        {
            // Query parameter values reach the serializer as ToStream<object>(value).
            object parameter = new Bridge("Kessler");

            string json = ReadAll(serializer.ToStream(parameter));

            Assert.That(json, Is.EqualTo("\"Kessler\""));
        }

        [Test]
        public void ToStream_FamilyDateDeclaredAsObject_WritesDateString()
        {
            object parameter = new FamilyDate("1888", Month.Feb, 20);

            string json = ReadAll(serializer.ToStream(parameter));

            Assert.That(json, Is.EqualTo("\"20 Feb 1888\""));
        }

        // =====================================================================
        // ToStream — unsupported input
        // =====================================================================

        [Test]
        public void ToStream_RawString_ThrowsNotSupportedException()
        {
            object parameter = "Kessler";

            Assert.Throws<NotSupportedException>(() => serializer.ToStream(parameter));
        }

        [Test]
        public void ToStream_Dictionary_ThrowsNotSupportedException()
        {
            IDictionary<string,BridgeValue> document = CreatePersonAttributes();

            Assert.Throws<NotSupportedException>(() => serializer.ToStream(document));
        }

        [Test]
        public void ToStream_Null_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => serializer.ToStream<object>(null!));
        }

        // =====================================================================
        // FromStream<IBridge>
        // =====================================================================

        [Test]
        public void FromStream_IBridge_ReadsObjectIntoDictionary()
        {
            string json = $"{{\"id\":\"{PersonId}\",\"birthFamilyName\":\"Kessler\",\"birthName\":\"Colby Bryan Kessler\",\"birthDate\":\"20 Feb 1888\",\"deceasedDate\":null}}";

            IBridge bridge = serializer.FromStream<IBridge>(ToJsonStream(json));

            Assert.That(bridge.Value.AsObject, Is.EqualTo(CreatePersonAttributes()));
        }

        [Test]
        public void FromStream_IBridge_KeepsCosmosSystemProperties()
        {
            string json = "{\"id\":\"a\",\"_rid\":\"rid==\",\"_self\":\"dbs/x/\",\"_etag\":\"\\\"0000\\\"\",\"_attachments\":\"attachments/\",\"_ts\":1729000000}";

            IDictionary<string,BridgeValue> document = serializer.FromStream<IBridge>(ToJsonStream(json)).Value.AsObject;

            Assert.That((string)document["_rid"], Is.EqualTo("rid=="));
            Assert.That((string)document["_self"], Is.EqualTo("dbs/x/"));
            Assert.That((string)document["_etag"], Is.EqualTo("\"0000\""));
            Assert.That((string)document["_attachments"], Is.EqualTo("attachments/"));
            Assert.That((long)(Number)document["_ts"], Is.EqualTo(1729000000L));
        }

        [Test]
        public void FromStream_IBridge_ReadsNestedObjectsAndArrays()
        {
            string json = "{\"id\":\"a\",\"aliases\":[\"TJ\",\"Tod\"],\"residence\":{\"city\":\"Dayton\",\"years\":[1990,2000]}}";

            IDictionary<string,BridgeValue> document = serializer.FromStream<IBridge>(ToJsonStream(json)).Value.AsObject;

            Assert.That(document["aliases"].AsArray.Select(a => (string)a), Is.EqualTo(new[] { "TJ", "Tod" }));
            IDictionary<string,BridgeValue> residence = document["residence"].AsObject;
            Assert.That((string)residence["city"], Is.EqualTo("Dayton"));
            Assert.That(residence["years"].AsArray.Select(y => (int)(Number)y), Is.EqualTo(new[] { 1990, 2000 }));
        }

        [Test]
        public void FromStream_IBridge_JsonNullBody_ReturnsBridgeWithNullValue()
        {
            IBridge bridge = serializer.FromStream<IBridge>(ToJsonStream("null"));

            Assert.That(bridge, Is.Not.Null);
            Assert.That(bridge.Value.IsNull, Is.True);
        }

        [Test]
        public void FromStream_IBridge_EmptyStream_ThrowsJsonException()
        {
            Assert.Throws<JsonException>(() => serializer.FromStream<IBridge>(ToJsonStream("")));
        }

        // =====================================================================
        // FromStream<IBridge[]> — query pages
        // =====================================================================

        [Test]
        public void FromStream_IBridgeArray_PreservesOrderAndCount()
        {
            string json = "[{\"id\":\"a\"},{\"id\":\"b\"},{\"id\":\"c\"}]";

            IBridge[] page = serializer.FromStream<IBridge[]>(ToJsonStream(json));

            Assert.That(page.Select(b => (string)b.Value.AsObject["id"]), Is.EqualTo(new[] { "a", "b", "c" }));
        }

        [Test]
        public void FromStream_IBridgeArray_EmptyArray_ReturnsEmptyArray()
        {
            IBridge[] page = serializer.FromStream<IBridge[]>(ToJsonStream("[]"));

            Assert.That(page, Is.Empty);
        }

        [Test]
        public void FromStream_IBridgeArray_ObjectBody_ThrowsInvalidCastException()
        {
            Assert.Throws<InvalidCastException>(() => serializer.FromStream<IBridge[]>(ToJsonStream("{\"id\":\"a\"}")));
        }

        // =====================================================================
        // FromStream — unsupported types
        // =====================================================================

        [Test]
        public void FromStream_Person_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => serializer.FromStream<Person>(ToJsonStream("{\"id\":\"a\"}")));
        }

        [Test]
        public void FromStream_Bridge_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => serializer.FromStream<Bridge>(ToJsonStream("{\"id\":\"a\"}")));
        }

        [Test]
        public void FromStream_Object_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => serializer.FromStream<object>(ToJsonStream("{\"id\":\"a\"}")));
        }

        [Test]
        public void FromStream_ListOfIBridge_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => serializer.FromStream<List<IBridge>>(ToJsonStream("[]")));
        }

        // =====================================================================
        // FromStream — stream disposal
        // =====================================================================

        [Test]
        public void FromStream_IBridge_DisposesStream()
        {
            MemoryStream stream = ToJsonStream("{\"id\":\"a\"}");

            serializer.FromStream<IBridge>(stream);

            Assert.That(stream.CanRead, Is.False);
        }

        [Test]
        public void FromStream_IBridgeArray_DisposesStream()
        {
            MemoryStream stream = ToJsonStream("[{\"id\":\"a\"}]");

            serializer.FromStream<IBridge[]>(stream);

            Assert.That(stream.CanRead, Is.False);
        }

        [Test]
        public void FromStream_UnsupportedType_StillDisposesStream()
        {
            MemoryStream stream = ToJsonStream("{\"id\":\"a\"}");

            Assert.Throws<NotSupportedException>(() => serializer.FromStream<Person>(stream));
            Assert.That(stream.CanRead, Is.False);
        }

        // =====================================================================
        // Round trip
        // =====================================================================

        [Test]
        public void RoundTrip_Person_ProducesEqualDictionaryAndEqualPerson()
        {
            Person original = new(CreatePersonAttributes());

            IBridge bridge = serializer.FromStream<IBridge>(serializer.ToStream(original));
            IDictionary<string,BridgeValue> document = bridge.Value.AsObject;

            Assert.That(document, Is.EqualTo(CreatePersonAttributes()));
            Assert.That(new Person(document), Is.EqualTo(original));
        }

        [Test]
        public void RoundTrip_AdditionalAttributes_ArePreserved()
        {
            Dictionary<string,BridgeValue> attributes = CreatePersonAttributes();
            attributes["nickname"] = "CB";
            attributes["isVerified"] = true;
            attributes["generation"] = (Number)3L;
            Person original = new(attributes);

            IDictionary<string,BridgeValue> document = serializer.FromStream<IBridge>(serializer.ToStream(original)).Value.AsObject;

            Assert.That(document, Is.EqualTo(attributes));
        }
    }
}
