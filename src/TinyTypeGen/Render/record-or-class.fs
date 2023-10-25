module TinyTypeGen.Render.RecordOrClass

open TinyTypeGen
open TinyTypeGen.Renderer.RenderFunctions

let renderRecord (t: System.Type) (env: Env) =
  let isAnonymous = isAnonymousRecord t

  if t.IsGenericType && not t.IsGenericTypeDefinition && not isAnonymous then
    failwith "A definition and value for a generic type that is not a generic type definition cannot be rendered"

  let callingModule = getModuleName t
  let name = env.GetName t

  let fields =
    t
    |> getProperties
    |> Seq.map (fun x -> renderPropertyNameAndDefinition callingModule x env)
    |> String.concat System.Environment.NewLine

  let genericArguments = genericArgumentList t

  $"""export type {name}{genericArguments} = {{
  {fields}
}}"""
