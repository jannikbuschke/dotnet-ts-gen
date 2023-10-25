module TinyTypeGen.Renderer.RenderFunctions

open System.Reflection
open TinyTypeGen

let renderPropertyNameAndDefinition (callingModule: string) (fieldInfo: PropertyInfo) (env: Env) =

  let propertyType = fieldInfo.PropertyType
  let signature = env.GetPropertySignature callingModule propertyType
  let nullableAttribute = typeof<System.Runtime.CompilerServices.NullableAttribute>

  let nullableContextAttribute =
    typeof<System.Runtime.CompilerServices.NullableContextAttribute>

  let hasNullableAttribute =
    fieldInfo.CustomAttributes
    |> Seq.exists (fun x ->
      not (propertyType.Name.StartsWith("Skippable"))
      && not (propertyType.Name.StartsWith("FSharpOption"))
      && x.AttributeType = nullableAttribute
      || x.AttributeType = nullableContextAttribute
    )

  let nullableSuffix = if hasNullableAttribute then " | null" else ""
  let propertyName = Utils.applyPropertyCasing env.PropertyCasing fieldInfo.Name
  $"""  {propertyName}: {signature}{nullableSuffix}"""

let ignoreList = [ typedefof<FSharpFunc<_, _>> ]

let isIgnored (x: System.Type) =

  let t =
    if x.IsGenericTypeDefinition then x
    else if x.IsGenericType then x.GetGenericTypeDefinition()
    else x

  let shouldBeIgnored = ignoreList |> List.exists (fun v -> v = t)
  shouldBeIgnored

let getProperties (t: System.Type) =
  // maybe ignore properties that have a NotMappedAttribute
  t.GetProperties(
    BindingFlags.Public ||| BindingFlags.Instance
  // ||| BindingFlags.Static
  )
  |> Seq.filter (fun x -> not (isIgnored x.PropertyType))
