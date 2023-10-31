namespace TsGen

open System

[<RequireQualifiedAccess>]
type HttpVerb =
  | POST
  | PATCH
  | GET
  | DELETE
  | PUT

type ApiEndpoint =
  { Request: Type
    Response: Type
    Method: HttpVerb
    Route: string }

type TsModule =
  { Name: string
    Types: System.Type list }
