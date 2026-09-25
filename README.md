# Family Tree Project
# Background
Initially, the family reunion committee is implementing a family tree in a word document and continually saving each update as PDF. After attending the reunion and looking at the physical copy of the tree, I thought to myself that I can automate this process by the applying the fundamemental concepts of Full-Stack Software programming. As a family reunion committee member, I am automating the family tree by writting a ASP.NET Core Full-Stack application that represents each family instance as a tree node in terms of Graph Theory as a MongoDB Collection record and in the DAO, Reading/Writing from a MongoDB Collection partitioned by family name as tree structure represented in Graph Theory.

# Resources
"2023 Pfingsten Book Alternate.docx": this is a word document that I am updating to save as a PDF.
"2023PfingstenBookAlternate.pdf": this is the file that is being uploaded for testing.
"appsettings.json": this is main settings of the project.
"firstEntrySample.json": shows a sample MongoDB Collection record

# Execution Types
The backend (`server/`) can run in four contexts, represented by the `ExecutionTypes` enum (`VirtualFamilyMuseumLibrary`) that every shared bootstrap method (see Extensions.cs below) is parameterized on:
- `API`: the ASP.NET Core Web API that will eventually host the Controller layer for this project. Not yet built.
- `Console`: a standalone console entry point. `VirtualFamilyMuseumScratch` currently plays this role informally — scratch code for trying things out and seeing output on the console — rather than a project formally wired up through `ExecutionTypes.Console`.
- `Functions`: an Azure Functions isolated-worker host. Not yet built.
- `Test`: `VirtualFamilyMuseumLibraryTest`, the NUnit test project — the one execution type with a fully working project today, bootstrapping its own `Host.CreateApplicationBuilder()` per test fixture.

Each execution type reads its Azure App Configuration/Key Vault endpoints from its own pair of environment variables — `FamilyConfiguration__Endpoint`/`FamilyVault__Endpoint` for API/Console/Functions, `FamilyConfigurationTest__Endpoint`/`FamilyVaultTest__Endpoint` for Test — so a local test run never points at the same configuration store as a real deployment.

# Extensions.cs
`VirtualFamilyMuseumLibrary/Extensions.cs` (top-level namespace — not to be confused with an aspect's own extensions class, e.g. `Drive/DriveExtensions.cs`, see Aspects below) holds the small set of static utility surface every aspect can use, regardless of which one it belongs to:
- `EN_DASH`: the single Unicode en-dash constant (`–`) used anywhere a date range needs rendering/parsing consistently (e.g. `FamilyDate`, or `TemplateLine.ToString()` in the Drive aspect) rather than each call site hardcoding its own dash character.
- `AddConstantStorePipeline(this IConfigurationBuilder, ExecutionTypes)`: wires Azure App Configuration and Key Vault into the configuration pipeline, picking which pair of endpoint environment variables to read based on the execution type (see Execution Types above). A no-op for whichever store's endpoint variable isn't set, so a context that doesn't need one doesn't fail startup over it.
- `AddFamilyInsights(this IHostApplicationBuilder, ExecutionTypes)`: wires Application Insights telemetry and logging, tagging every emitted telemetry item with a cloud role name equal to the execution type's own name (`API`, `Console`, `Functions`, `Test`) so multiple execution types reporting to the same Application Insights resource can be told apart. Also a no-op if `FamilyInsights:ConnectionString` isn't configured.

Both extension methods are aspect-agnostic — they don't know or care whether the caller is about to touch Drive, Serialization, or anything else; they only ever configure the host itself.

# Shared Models
`VirtualFamilyMuseumLibrary/Models` holds model types shared across more than one aspect of this project — as distinct from an aspect's own `Models` folder (e.g. `Drive/Models`, documented in that aspect's own child document; see Aspects below), which only that one aspect depends on.

- `DomainResult<T>`: the universal shape any domain-layer class in this project can hand back to its caller (a Controller, most often) to communicate an outcome without necessarily requiring a try/catch for every one. Carries a required `Message` (what happened, in caller-facing language) and an optional `Payload` (`T?` — whatever the operation actually produced, if anything). It's deliberately minimal and has no notion of success/failure built in on its own: an aspect that needs to distinguish more than one kind of outcome extends it rather than `DomainResult<T>` growing a status concept that would only make sense for some aspects and not others. The Drive aspect's `FamilyDriveResult<T>` (see Aspects) is the first such extension, adding its own `Status` of type `FamilyDriveResultStatuses`.
- `FamilyDate`: an immutable year (required) plus optional month and day — deliberately looser than `DateOnly`, since genealogical records routinely have partial dates (a year alone, or a year and month with no day) or even an uncertain year *range* (e.g. `"1940-1943"`). Implements `IBridge` (see the Serialization aspect below) so it has a uniform serializable representation without needing its own bespoke persistence logic. Its comparisons (`IComparable`, the relational operators) treat a missing month/day as sorting before a present one, and a year range as spanning between its own min and max when compared against a specific year.
- `Month`: a plain `Jan`-`Dec` enum — `FamilyDate`'s month component, and the same type the Drive aspect's template PDF format renders a date's month as (see drive.md's Template Processing section).

# Aspects
This project's backend is organized into distinct **aspects** — self-contained areas of functionality, each with its own models, utilities, and (where relevant) repository/domain layers. An aspect substantial enough to need its own deep-dive gets a child document living alongside its own code rather than growing inside this README, so it stays next to what it describes and can be updated independently of everything else. Everything above this section — `DomainResult<T>`/`FamilyDate`/`Month`, `Extensions.cs`, `ExecutionTypes` — is shared *across* aspects rather than belonging to any single one.

- **Drive Aspect** — Azure Blob Storage: templates and images, their models, utilities, repository, and domain logic (`TemplateReader`, `TemplateWriter`, `FamilyDriveService`). Documented in full in [`server/VirtualFamilyMuseumLibrary/Drive/drive.md`](server/VirtualFamilyMuseumLibrary/Drive/drive.md).
- **Serialization Aspect**: `VirtualFamilyMuseumLibrary/Serialization` is the **single source of truth for JSON communication** across the whole stack: Cosmos DB, Neo4j, C# .NET 9, and React. **C# .NET 9 is the central hub.** Every value passes through the library on its way anywhere else, and the three edges never talk to each other directly:
  - **Cosmos DB ⇄ C# .NET 9:** documents are stored as JSON, so this edge is a direct mapping.
  - **Neo4j ⇄ C# .NET 9:** vertices only hold flat primitive properties, so this edge flattens nested values and drops nulls.
  - **React ⇄ C# .NET 9:** the Web API sends and receives camelCase JSON using the same serializer settings the stores use.

  Each domain type (e.g. `FamilyDate`) describes itself once as a JSON value through the `IBridge`/`Bridge`/`BridgeValue` layer. Every edge translates from that one value, rather than each type having its own mapping for each destination. `BridgeSerializer` is the only code that reads or writes JSON text, and `SerializationExtensions.GetOptions` is the one set of serializer options everything shares. So far only the hub side is built. The Cosmos DB and Neo4j adapters, the Web API, and the React source are not, and `serialization.md` records the rules they must follow.

  This is only an overview. The details are in [`server/VirtualFamilyMuseumLibrary/Serialization/serialization.md`](server/VirtualFamilyMuseumLibrary/Serialization/serialization.md): the models, the converter, the full type-mapping table for all four destinations, the rules for the React client, and the currently known limitations.
- More aspects will be documented here, each with their own child document, as they're built.

# Getting Started with Create React App

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

The page will reload if you make edits.\
You will also see any lint errors in the console.

### `npm test`

Launches the test runner in the interactive watch mode.\
See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### `npm run build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.\
Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### `npm run eject`

**Note: this is a one-way operation. Once you `eject`, you can’t go back!**

If you aren’t satisfied with the build tool and configuration choices, you can `eject` at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (webpack, Babel, ESLint, etc) right into your project so you have full control over them. All of the commands except `eject` will still work, but they will point to the copied scripts so you can tweak them. At this point you’re on your own.

You don’t have to ever use `eject`. The curated feature set is suitable for small and middle deployments, and you shouldn’t feel obligated to use this feature. However we understand that this tool wouldn’t be useful if you couldn’t customize it when you are ready for it.

## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).
