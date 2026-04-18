# AGENTS.md — TinyTypeGen Repository Guide

## Project Overview

**TinyTypeGen** is an F# library that generates TypeScript type definitions from .NET/F# types at runtime using reflection. It targets **net9.0** and is published as a set of NuGet packages:

- `TinyTypeGen` — core library
- `TinyTypeGen.AspNetCore` — ASP.NET Core endpoint discovery
- `TinyTypeGen.Giraffe` — Giraffe integration
- `TinyTypeGen.SignalR` — SignalR hub support

The build system uses **NUKE** (build orchestration in `build/Build.cs`). Tests use **xUnit** + **Expecto** + **FsCheck** in F#.

---

## Build, Lint & Test Commands

### Using the NUKE build wrapper (preferred)

```bash
# On Linux/macOS
./build.sh <Target>

# On Windows
./build.cmd <Target>
```

### Common NUKE targets

```bash
./build.sh Compile          # Restore + build entire solution (Release)
./build.sh UnitTests        # Run unit tests only (tests/UnitTests/)
./build.sh IntegrationTests # Run integration tests + pnpm format + pnpm build
./build.sh Tests            # Run all tests (UnitTests + IntegrationTests)
./build.sh Pack             # Create NuGet packages in output/
./build.sh Publish          # Pack + push to NuGet (requires .env.local with API key)
./build.sh Format           # Format all F# source files with Fantomas
```

### Direct `dotnet` commands (for faster iteration)

```bash
# Build the entire solution
dotnet build tiny-type-gen.sln -c Release

# Run all unit tests
dotnet test tests/UnitTests/TinyTypeGen.Test.fsproj

# Run a SINGLE test by name (xUnit filter syntax)
dotnet test tests/UnitTests/TinyTypeGen.Test.fsproj --filter "FullyQualifiedName~Render record with primitive option"

# Run tests in a specific module/file
dotnet test tests/UnitTests/TinyTypeGen.Test.fsproj --filter "FullyQualifiedName~Test.Record"

# Run integration tests
dotnet test tests/IntegrationTests/IntegrationTests.fsproj

# Format F# source with Fantomas
dotnet tool run fantomas .
```

### TypeScript integration test (pnpm)

The integration test runner generates TypeScript files and then validates them:

```bash
cd tests/IntegrationTests/examples
pnpm run format   # Prettier format check
pnpm run build    # tsc type-check
```

---

## Code Style Guidelines

### Language & Framework

- **Primary language**: F# (`.fs`) targeting net9.0
- **Build tooling**: C# (`build/Build.cs`) using NUKE
- **Formatting tool**: [Fantomas](https://fsprojects.github.io/fantomas/)
- **F# JSON**: `FSharp.SystemTextJson` (`System.Text.Json.Serialization`)

### Indentation & Formatting

Enforced by `.editorconfig` and Fantomas:

- **Indent size**: 2 spaces (no tabs) for all F# files
- **Line endings**: LF (`\n`)
- **Trailing whitespace**: trimmed
- **Multiline bracket style**: `aligned`
- **Records**: always multiline (one item per line), formatted:
  ```fsharp
  type MyRecord =
    {
      Id: Guid
      Name: string
    }
  ```
- **Bar before discriminated union declaration**: required
  ```fsharp
  type MyDu =
    | CaseA
    | CaseB of int
  ```
- **Multiline lambda closing newline**: yes
- **Blank lines around nested multiline expressions**: no

### Module & File Structure

- Each `.fs` file starts with either `module <Name>` or `namespace <Name>`
- **Compilation order matters in F#** — files listed in `.fsproj` must be in dependency order
- `[<AutoOpen>]` modules expose helpers globally within their assembly
- `[<RequireQualifiedAccess>]` is used on union types that should be prefixed: `HttpVerb.GET`
- Private implementation helpers go in a `module private Helper` inside the file

### Imports (open statements)

- Place `open` statements at the top, after the module/namespace declaration
- Group by: BCL (`System.*`), third-party, then project-internal
- Use `open type` to bring enum members into scope (e.g., `open type JsonUnionEncoding`)
- Do not wildcard-open modules unless they are `[<AutoOpen>]`

Example ordering:
```fsharp
module TinyTypeGen.Render.Unions

open System
open System.Reflection
open System.Text.Json.Serialization.TypeCache
open System.Text.Json.Serialization
open type JsonUnionEncoding
open Microsoft.FSharp.Reflection
open TinyTypeGen
```

### Naming Conventions

| Construct | Convention | Example |
|---|---|---|
| Types / Discriminated Unions | `PascalCase` | `TsModule`, `HttpVerb` |
| DU cases | `PascalCase` | `AdjacentTag`, `POST` |
| Record fields | `PascalCase` | `ApiEndpoint.Route` |
| Functions / values | `camelCase` | `getModuleName`, `renderDu` |
| Private helpers | `camelCase` | `renderFieldlessCaseDefinition` |
| Type parameters | single uppercase letter or short name | `'a`, `'t` |
| Module-level `let` bindings | `camelCase` | `defaultJsonUnionEncoding` |

> **TypeScript output** uses camelCase property names (enforced via `Utils.camelize` / `JsonNamingPolicy.CamelCase`).

### Types & Signatures

- Prefer explicit type annotations on public-facing functions
- Use `option` types instead of nullable references for F# data
- Use `string | null` for C#-interop nullable strings (net6+ nullable enabled)
- The project enables `<Nullable>enable</Nullable>` in the core project
- Prefer `Result<'ok, 'err>` over exceptions for recoverable errors
- Use `failwith` or `failwithf` for unrecoverable/programmer errors in core logic

### Error Handling

- Throw `failwith "descriptive message"` for states that indicate a bug (unexpected code paths)
- Use `sprintf` / string interpolation (`$"..."`) for error messages that include runtime values
- Avoid `try/catch` except at boundaries (e.g., Scriban template rendering in `unions.fs`)
- Integration test helpers return `Result<string, string>` (Ok/Error) for process execution

### Pattern Matching

- Prefer exhaustive `match` expressions
- Use active patterns (e.g., `(|Adjacent|_|)`) to decompose flags/enums cleanly
- Inline pattern matching in pipelines is idiomatic:
  ```fsharp
  |> match env.Encoding with
     | Adjacent -> sprintf "{ Case: \"%s\", %s }" unionCase.Name
     | External -> sprintf "{ %s: %s }" unionCase.Name
     | _ -> failwith "not supported"
  ```

### Pipelines & Composition

- Prefer pipeline operators (`|>`, `>>`) over intermediate variables
- Function composition (`>>`) is used for string processing pipelines:
  ```fsharp
  let clean =
    normalizeLineFeeds
    >> removeSuccessiveLineFeeds
    >> removeSuccessiveWhiteSpace
    >> trim
  ```

### Records

- Always use multiline record construction (enforced by Fantomas config):
  ```fsharp
  {
    Name = "foo"
    Types = []
    Dependencies = []
  }
  ```
- Use `{ record with Field = newValue }` for copy-and-update

### Classes & Object Expressions

- Prefer F# records and DUs over classes where possible
- Use object expressions (`{ new IFoo with ... }`) for single-use interface implementations
- Builder pattern (mutable `let mutable`) is used in `GeneratorBuilder` for C#-friendly API

---

## Testing Conventions

### Unit Tests (`tests/UnitTests/`)

- Framework: **xUnit** (attributes) + **Expecto** (assertions)
- Test functions are `let` bindings decorated with `[<Fact>]`
- Use backtick names for readable test descriptions:
  ```fsharp
  [<Fact>]
  let ``Render simple record`` () = ...
  ```
- Use `definition` helper (from `Utils.fs`) to render a type and compare output
- Use `Expect.equal expected actual` (note: expected first) for exact string comparison
- Use `Expect.similar` for whitespace-insensitive comparison

### Integration Tests (`tests/IntegrationTests/`)

- Generate TypeScript files from real .NET types
- Validate generated TS compiles with `pnpm run build` (tsc)
- Uses `WebApplicationFactory` to test ASP.NET Core endpoint discovery

---

## Key Architectural Patterns

1. **`GeneratorBuilder`** — fluent builder for configuring type generation
2. **`Env` interface** — passed through rendering functions to avoid global state
3. **`PredefinedTypes`** — override how specific .NET types are rendered in TypeScript
4. **`TsModuleWithDeps`** — types grouped by namespace, with dependency tracking for import generation
5. **Scriban templates** — optional `.scriban` files for customizing TypeScript output
6. **Artifact + caching** — rendered modules are hashed; unchanged files are not rewritten
