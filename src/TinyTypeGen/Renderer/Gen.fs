module TsGen.Gen

open System.Reflection
open System.Text.Json.Serialization
open TsGen
open Microsoft.FSharp.Reflection
open TypeCache
open TinyTypeGen.Renderer.RenderFunctions

let defaultJsonUnionEncoding =
  JsonUnionEncoding.AdjacentTag
  ||| JsonUnionEncoding.UnwrapSingleFieldCases
  ||| JsonUnionEncoding.UnwrapRecordCases
  ||| JsonUnionEncoding.UnwrapOption
  ||| JsonUnionEncoding.UnwrapSingleCaseUnions
  ||| JsonUnionEncoding.AllowUnorderedTag

let supportedJsonUnionEncoding = defaultJsonUnionEncoding

let init (defaultTypes: PredefinedTypes.PreDefinedTypes) (jsonUnionEncoding: JsonUnionEncoding) =
  let renderType (t: System.Type) (strategy: RenderStrategy) =
    let kind = getKind t
    let predefinedDefinitionAndValue =
      PredefinedTypes.tryPredefinedType defaultTypes t
      |> Option.map (fun value ->
        if t.IsGenericType && not t.IsGenericTypeDefinition then
          renderPredefinedType (t.GetGenericTypeDefinition()) value strategy
        else
          renderPredefinedType t value strategy)

    predefinedDefinitionAndValue
    |> Option.defaultValue (
      match kind with
      | TypeKind.List ->
        if t.IsGenericType && not t.IsGenericTypeDefinition then
          ""
        else
          let def = $"""export type FSharpList<T> = Array<T>"""

          let value =
            "export var defaultFSharpList: <T>(t:T) => FSharpList<T> = <T>(t:T) => []"

          renderDefinitionAndOrValue def //value strategy

      | TypeKind.Option -> TsGen.Option.render jsonUnionEncoding strategy
      | TypeKind.Record ->
        if t.IsGenericType && not t.IsGenericTypeDefinition then
          if isAnonymousRecord t then
            renderRecord t strategy
          else
            ""
        else
          let result = renderRecord t strategy
          result
      | TypeKind.Union -> renderDu t strategy
      | TypeKind.Array ->
        // Maybe not good
        let name = getName t
        let elementType = t.GetElementType()
        let elModuleName, elName = getSignature elementType

        let elementTypeName =
          if elModuleName = getModuleName t then
            elName
          else
            elModuleName + "." + elName

        let definition =
          $"""export type {name} = Array<{elementTypeName}> // fullname {t.FullName}"""

        let value = $"""export var default{name}: <T>(t:T) => {name}<T> = <T>(t:T) => []"""

        renderDefinitionAndOrValue definition //value strategy

      | TypeKind.Map ->
        //if key type is string, use normal object
        let definition, value =
          if not t.IsGenericTypeDefinition
             && t.GenericTypeArguments.[0] = typeof<string> then
            """export type FSharpStringMap<TValue> = { [key: string ]: TValue }""",
            """export var defaultFSharpStringMap: <TValue>(t:string,tValue:TValue) => FSharpStringMap<TValue> = <TValue>(t:string,tValue:TValue) => ({})"""
          else
            """export type FSharpMap<TKey, TValue> = [TKey,TValue][]""",
            """export var defaultFSharpMap: <TKey, TValue>(tKey:TKey,tValue:TValue) => FSharpMap<TKey, TValue> = <TKey, TValue>(tKey:TKey,tValue:TValue) => []"""

        renderDefinitionAndOrValue definition //value strategy
      | TypeKind.Enum ->
        let name = getName t

        let values =
          t.GetEnumNames()
          |> Array.map (fun v -> $"\"{v}\"")
          |> Array.toList

        $"""export type {name} = {values |> String.concat " | "}
export const {name}_AllValues = [{values |> String.concat ", "}] as const
export const default{name}: {name} = {values |> List.head}
"""
      | TypeKind.Tuple
      | TypeKind.Set
      | TypeKind.Other ->
        if t.IsGenericParameter then
          ""
        else
          // probably a class, try to use same strategy as record
          renderRecord t strategy
   )
  {| renderType = renderType
     renderStubValue = renderStubValue |}