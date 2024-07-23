# TinyTypeGen

[![Nuget](https://img.shields.io/nuget/v/TinyTypegen?logo=nuget)](https://nuget.org/packages/TinyTypegen)

This library can generate TypeScript types for C# and F# types or more precisely for their corresponding JSON serialized data. Thus it can be used in JavaScript/TypeScript clients to have a strongly typed interface to a dotnet backend.

F# types like records (including anonymous records) and unions as well as F# collections like `list<'T>`, `Map<'T>` and `Set<'T>` are supported.

# Get started

`dotnet add package TinyTypeGen --version 1.0.0.21-alpha`

```cs
var builder = new TinyTypeGen.GeneratorBuilder();
builder.AddTypes([typeof(FSharpResult<,>), typeof(MyType)])
builder.AddEndpoints(MyModule.endpoints);
var generator = builder.Build();
generator.RenderTypesToDirectory("../my-client/src/client/");
generator.RenderApiToFile("../my-client/src/client/api.ts");
```