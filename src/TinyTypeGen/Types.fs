namespace TinyTypeGen

open System
open System.Text.Json.Serialization

[<RequireQualifiedAccess>]
type HttpVerb =
  | POST
  | PATCH
  | GET
  | DELETE
  | PUT

[<RequireQualifiedAccess>]
type EndpointKind =
  | Query
  | Mutation

// react query
type ClientGcDefault =
  | Infinity
  | Timespan of TimeSpan
  | Undefined

type ApiEndpoint =
  {
    Request: Type
    Response: Type
    ResponseNullable: bool
    Method: HttpVerb
    Route: string
    Kind: EndpointKind
  }

type TsModule =
  {
    Name: string
    OriginalNamespacename: string
    Types: Type list
  }

type TsModuleWithDeps =
  {
    Name: string
    OriginalNamespacename: string
    Types: Type list
    Dependencies: Type list
  }

type PredefinedType =
  {
    InlineDefaultValue: string option
    Name: string option
    Definition: string option
    Signature: string option
    Dependencies: Type list
  }

  static member New definition =
    {
      InlineDefaultValue = None
      Name = None
      Definition = Some definition
      Signature = None
      Dependencies = []
    }
  static member New(definition, signature) =
    {
      InlineDefaultValue = None
      Name = None
      Definition = Some definition
      Signature = Some signature
      Dependencies = []
    }

type PreDefinedTypes = Collections.Generic.Dictionary<Type, PredefinedType>
type GetSignature = Type -> string * string
type GetName = Type -> string

[<RequireQualifiedAccess>]
type PropertyCasing =
  | CamelCase
  | PascalCase
  | SnakeCase

type Env =
  abstract member GetFullName: Type -> string
  abstract member GetSignature: GetSignature
  abstract member GetName: GetName
  abstract member GetPropertySignature: string | null -> Type -> string
  abstract member GetDependencies: Type -> Type list
  abstract member IsOverridden: Type -> bool
  abstract member RenderOverridden: Type -> string
  abstract member Encoding: JsonUnionEncoding
  abstract member PreDefinedTypes: PreDefinedTypes
  abstract member PropertyCasing: PropertyCasing
