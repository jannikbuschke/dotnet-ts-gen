# TinyTypeGen

Generates TypeScript types from C# and F# .NET types. An opinionated alternative to OpenAPI for projects using `System.Text.Json` or `FSharp.SystemTextJson`.

## Quick start

```sh
dotnet add package TinyTypeGen
```

```fsharp
// Render a single type to a string — no setup needed
let ts = Generator.RenderType(typedefof<MyRecord>)

// Or write all types for a project to a directory
GeneratorBuilder()
  .AddTypes([| typedefof<MyRecord>; typedefof<MyUnion> |])
  .Build()
  .RenderAll("../frontend/src/generated/")
```

That's it. By default, properties are camelCase and F# DUs use `AdjacentTag` encoding to match the standard `FSharp.SystemTextJson` configuration.

---

## Features

- F# Discriminated Unions (via FSharp.SystemTextJson)
- F# record and anonymous types
- Generic types and nullable reference types
- Flattened type hierarchies
- API endpoint generation (ASP.NET Core, Giraffe, SignalR)
- Property casing: `CamelCase`, `PascalCase`, `SnakeCase`
- Customization via [Scriban](https://github.com/scriban/scriban) templates

## Packages

| Package | Description |
|---|---|
| `TinyTypeGen` | Core type generator |
| `TinyTypeGen.AspNetCore` | ASP.NET Core API Explorer integration |
| `TinyTypeGen.Giraffe` | Giraffe endpoint helpers |
| `TinyTypeGen.SignalR` | SignalR hub type generation |

---

## Configuration

All configuration goes through `GeneratorBuilder`:

```fsharp
GeneratorBuilder()
  .AddTypes([| typedefof<MyRecord> |])
  .WithCasing(PropertyCasing.SnakeCase)        // CamelCase (default), PascalCase, SnakeCase
  .WithEncoding(JsonUnionEncoding.FSharpLuLike) // DU encoding
  .IgnoreNamespaces([ "MyApp.Internal" ])
  .AddOverrides(typeof<MyType>, PredefinedType.New("string"))
  .WithMaxRecursionDepth(50)
  .Build()
  .RenderAll("../frontend/src/generated/")
```

### One-liner overloads

```fsharp
// With defaults
Generator.RenderType(typedefof<MyRecord>)

// With a specific DU encoding
Generator.RenderType(typedefof<MyRecord>, JsonUnionEncoding.FSharpLuLike)

// With full configuration
Generator.RenderType(typedefof<MyRecord>, { Config.withDefaults() with PropertyCasing = PropertyCasing.PascalCase })
```

---

## API endpoints

### ASP.NET Core

```fsharp
let endpoints = TinyTypeGen.AspNetCore.getEndpoints services

GeneratorBuilder()
  .AddApi(
    { ApiTemplate    = Template.Default
      EndpointTemplate = Template.Default
      TargetFile     = "api.ts"
      Endpoints      = endpoints })
  .Build()
  .RenderAll("../frontend/src/generated/")
```

### Giraffe

Giraffe has no built-in metadata, so you declare request/response types explicitly via typed helpers:

```fsharp
open TinyTypeGen.Giraffe

let getUser    = queryEndpoint<GetUserRequest, UserResponse, ApiError> "/api/user" handler
let postUser   = mutationEndpoint<CreateUserRequest, UserResponse, ApiError> "/api/user" handler

let giraffeHandlers = [ getUser; postUser ] |> List.map (fst >> toGiraffeEndpoint)
let apiEndpoints    = [ getUser; postUser ] |> List.map fst
```

Available helpers:

| Helper | Verb | Input |
|---|---|---|
| `queryEndpoint` | GET | query string |
| `queryEndpointWithoutInput` | GET | none |
| `mutationEndpoint` | POST | JSON body |
| `paginatedQueryEndpoint` | GET | query string + pagination headers |

### SignalR

```fsharp
open TinyTypeGen.SignalR

let hub = createHub env getDeps (typeof<MyHub>, "hubs/my-hub.ts", "/hubs/my") |> Result.get

GeneratorBuilder()
  .AddHub(hub)
  .Build()
  .RenderAllIncludingHubs("../frontend/src/generated/", [ hub ])
```

---

## F# Discriminated Unions

The DU encoding is set via `JsonUnionEncoding` from `FSharp.SystemTextJson` and controls how cases are serialized.

### Layout

| Tag | Example JSON |
|---|---|
| `AdjacentTag` (default) | `{ "Case": "MyCase", "Fields": ... }` |
| `InternalTag` | `{ "MyCase": ... }` |
| `ExternalTag` | `["MyCase", ...]` |

Preset combinations: `Default`, `FSharpLuLike`, `NewtonsoftLike`, `ThothLike`.

### Limitations

- `[<JsonName>]` and other JSON attributes are not supported
- Custom `Case` / `Fields` tag names are not supported
- `Untagged`, `Inherit`, `UnwrapRecordCases` with generics are not supported

---

## Custom templates

```fsharp
open TinyTypeGen.Config

GeneratorBuilder()
  .AddApi(
    { ApiTemplate    = Template.OfFile "my-api.scriban"
      EndpointTemplate = Template.EmbeddedTemplate ApiTemplateWithTanstackQuery
      TargetFile     = "api.ts"
      Endpoints      = endpoints })
  .Build()
  .RenderAll("../frontend/src/generated/")
```

Template sources: `Template.Default` · `Template.EmbeddedTemplate` · `Template.OfString` · `Template.OfFile`

---

## Recommended JSON setup

TinyTypeGen works best when the app-wide JSON configuration is consistent. Typical ASP.NET Core setup:

```csharp
services
  .AddControllers()
  .AddJsonOptions(options =>
  {
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(jsonFsharpConverter);
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
  });
```
