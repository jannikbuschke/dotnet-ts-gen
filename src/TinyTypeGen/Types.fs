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

type RenderStrategy =
  | RenderDefinitionAndValue
  | RenderDefinition
  | RenderValue

[<AutoOpen>]
module Render = 
  let renderDefinitionAndOrValue definition value strategy =
    match strategy with
    | RenderValue -> value
    | RenderDefinition -> definition
    | RenderDefinitionAndValue -> definition + System.Environment.NewLine + System.Environment.NewLine + value
