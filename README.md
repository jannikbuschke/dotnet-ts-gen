# TinyTypeGen

[![Nuget](https://img.shields.io/nuget/v/TinyTypegen?logo=nuget)](https://nuget.org/packages/TinyTypegen)

This library can generate TypeScript types for C# and F# types or more precisely for their corresponding JSON serialized data. Thus it can be used in JavaScript/TypeScript clients to have a strongly typed interface to a dotnet backend.

F# types like records (including anonymous records) and unions as well as F# collections like `list<'T>`, `Map<'T>` and `Set<'T>` are supported.
