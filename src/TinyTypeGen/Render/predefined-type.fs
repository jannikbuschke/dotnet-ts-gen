module TinyTypeGen.Render.PredefinedType

open TinyTypeGen

let renderPredefinedType (t: System.Type) (predefined: PredefinedType) =
  let name = predefined.Name |> Option.defaultValue (t.Name.Split("`")[0])

  if t.IsGenericType then
    let genericArguments = genericArgumentList t

    $"""export type {name}{genericArguments} = {(predefined.Definition
                                                 |> Option.defaultValue "unknown // renderPredefinedTypeFromDefaultValue (generic)")}"""
  else
    $"""export type {name} = {(predefined.Definition |> Option.defaultValue "any")}"""
