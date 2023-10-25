module TinyTypeGen.Gen

open System.Text.Json.Serialization
open TypeCache
open TinyTypeGen.Render

let renderType (t: System.Type) =
  fun (env: Env) ->
    let kind = getKind t

    if env.IsOverridden t then
      env.RenderOverridden t
    else

      match kind with
      | TypeKind.List ->
        if t.IsGenericType && not t.IsGenericTypeDefinition then
          ""
        else
          """export type FSharpList<T> = Array<T>"""
      | TypeKind.Option -> Option.render env.Encoding
      | TypeKind.Record ->
        if t.IsGenericType && not t.IsGenericTypeDefinition then
          if isAnonymousRecord t then
            // RecordOrClass.renderRecord t config.PredefinedTypes
            RecordOrClass.renderRecord t env
          else
            ""
        else
          RecordOrClass.renderRecord t env
      | TypeKind.Union -> Unions.renderDu t env
      | TypeKind.Array ->
        let name = env.GetName t
        let elementType = t.GetElementType()
        let elModuleName, elName = env.GetSignature elementType

        let elementTypeName =
          if elModuleName = getModuleName t then
            elName
          else
            elModuleName + "." + elName

        $"""export type {name} = Array<{elementTypeName}> // fullname {t.FullName}"""

      | TypeKind.Map ->

        """export type FSharpMap<TKey, TValue> = TKey extends string
  ? string extends TKey
    ? { [key: string]: TValue }
    : [[TKey, TValue]]
  : [[TKey, TValue]]

  """

      | TypeKind.Enum ->
        let name = env.GetName t

        let values = t.GetEnumNames() |> Array.map (fun v -> $"\"{v}\"") |> Array.toList

        $"""export type {name} = {values |> String.concat " | "}
export const {name}_AllValues = [{values |> String.concat ", "}] as const
export const default{name}: {name} = {values |> List.head}
export function is{name}(value: any): value is {name} {{
  return {name}_AllValues.includes(value)
}}"""
      | TypeKind.Tuple
      | TypeKind.Set
      | TypeKind.Other ->
        if t.IsGenericParameter then
          ""
        else
          // probably a class, try to use same strategy as record
          RecordOrClass.renderRecord t env
