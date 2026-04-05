## 1.0.0-alpha.190 (785647e239acc6627d9b3118236bfd30d34830eb)

### Features
- separate query and mutation endpoints in default template

### Bug Fixes
- **routeWithMetadata**: fix not all metadata is applied

## 1.0.0-alpha.189 (53cb9e5f535ec36b34364a2dd9ec52d1867f2214)

### Features

- F# discriminated unions (AdjacentTag/FSharpLu/ExternalTag encodings)
- F# records, anonymous records, generic and nullable types
- C# classes and enums
- Property casing: CamelCase (default), PascalCase, SnakeCase
- Type ignore/override via GeneratorBuilder
- Topological sort for correct module dependency ordering
- Skip unchanged modules (stable hash)
- ASP.NET Core API Explorer integration (TinyTypeGen.AspNetCore)
- Giraffe endpoint helpers (TinyTypeGen.Giraffe)
- SignalR hub type generation (TinyTypeGen.SignalR)
- Scriban template customization
- TanStack Query template included
- Built-in types: DateOnly, TimeOnly, ValueTuple, FSharpMap, Guid and moret
