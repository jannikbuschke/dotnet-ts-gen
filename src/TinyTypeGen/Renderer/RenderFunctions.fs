module TinyTypeGen.Renderer.RenderFunctions

open System.Reflection
open System.Text.Json.Serialization
open TsGen
open Microsoft.FSharp.Reflection
open TypeCache

let renderPropertyNameAndDefinition (callingModule: string) (fieldInfo: PropertyInfo) (defaultTypes:PredefinedTypes.PreDefinedTypes)=
  let signature = getPropertySignature callingModule fieldInfo.PropertyType defaultTypes
  let name = fieldInfo.Name
  let result = $"""  {Utils.camelize name}: {signature}"""
  result

let renderSingleFieldUnionCaseDefinition (callingModule: string) (case: UnionCaseInfo) (fieldInfo: PropertyInfo)(defaultTypes:PredefinedTypes.PreDefinedTypes) =
  $"""{{ Case: "{case.Name}", Fields: {getPropertySignature callingModule fieldInfo.PropertyType defaultTypes} }}"""

let renderMultiFieldUnionCaseDefinition
    (callingModule: string)
    (outerDu: System.Type)
    (case: UnionCaseInfo)
    (fieldInfo: PropertyInfo list)
    (defaultValues:PredefinedTypes.PreDefinedTypes)
    =
    let fields =
      fieldInfo
      |> List.map (fun v ->
        (Utils.camelize v.Name)
        + ": "
        + getDuPropertySignature callingModule v.PropertyType defaultValues)
      |> String.concat ", "

    $"""{{ Case: "{case.Name}", Fields: {{ {fields} }} }}"""
let getFieldCaseName (name: string) (case: UnionCaseInfo) =
    let name = $"{name}_Case_{case.Name}"
    name

let getSingleFieldCaseSignature (name: string) (singleField: PropertyInfo) (case: UnionCaseInfo) =
    let name = getFieldCaseName name case

    let genericParameterPostfix =
      if singleField.PropertyType.IsGenericParameter then
        "<" + singleField.PropertyType.Name + ">"
      else
        ""

    let name = $"{name}{genericParameterPostfix}"
    name
let getMultiFieldCaseSignature
    (outerDu: System.Type)
    (name: string)
    (fields: PropertyInfo list)
    (case: UnionCaseInfo)
    =
    let name = getFieldCaseName name case
    let genericParameterPostfix = genericArgumentList outerDu
    let name = $"{name}{genericParameterPostfix}"
    name


let renderDu (outerDu: System.Type) (strategy: RenderStrategy)(defaultTypes:PredefinedTypes.PreDefinedTypes)=

    let callingModule = getModuleName outerDu

    let name = getName outerDu
    let cases = (FSharpType.GetUnionCases outerDu) |> Seq.toList

    let caseNameLiteral =
      cases
      |> List.map (fun v -> $"\"{v.Name}\"")
      |> String.concat " | "

    let allCaseNames =
      cases
      |> List.map (fun v -> $"\"{v.Name}\"")
      |> String.concat ", "

    let renderedCaseDefinitions =
      match cases with
      | [] -> failwith "not yet implemented"
      | [ singleCase ] ->
        let caseFields = singleCase.GetFields() |> Seq.toList

        match caseFields with
        | [] -> $"""export type {singleCase.Name} = {singleCase.Name} // DU single case no fields"""
        | [ singleField ] ->
          let singleFieldCaseSignature =
            getSingleFieldCaseSignature name singleField singleCase

          let prop = getPropertySignature callingModule singleField.PropertyType defaultTypes
          $"""export type {singleFieldCaseSignature} = {prop}"""
        | _ -> failwith "not yet implemented"

      | cases ->
        let renderCase (case: UnionCaseInfo) =

          let caseFields = case.GetFields() |> Seq.toList

          match caseFields with
          | [] -> $"""export type {name}_Case_{case.Name} = {{ Case: "{case.Name}" }}"""
          | [ singleField ] ->
            let singleFieldCaseSignature = getSingleFieldCaseSignature name singleField case

            let singleFieldUnionCaseDefinition =
              renderSingleFieldUnionCaseDefinition callingModule case singleField defaultTypes

            $"""export type {singleFieldCaseSignature} = {singleFieldUnionCaseDefinition}"""
          | fields ->
            let singleFieldCaseSignature = getMultiFieldCaseSignature outerDu name fields case

            let multiFieldUnionCaseDefinition =
              renderMultiFieldUnionCaseDefinition callingModule outerDu case fields defaultTypes

            $"""export type {singleFieldCaseSignature} = {multiFieldUnionCaseDefinition}"""

        cases
        |> List.map renderCase
        |> String.concat System.Environment.NewLine

    let anonymousFunctionSignature =
      getAnonymousFunctionSignatureForDefaultValue outerDu

    (*let renderedCaseDefaultNamesAndValues =
      match cases with
      | [] -> failwith "todo"
      | [ singleCase ] ->
        let caseFields = singleCase.GetFields() |> Seq.toList

        match caseFields with
        | [] -> failwith "todo"
        | [ singleField ] ->
          let singleFieldCaseSignature = getFieldCaseName name singleCase
          let unwrappedValue = getDefaultValue callingModule singleField.PropertyType

          $"""export var default{singleFieldCaseSignature} = {unwrappedValue}"""
        | _ -> failwith "todo"
      | cases ->
        let renderCase (case: UnionCaseInfo) =
          let caseFields = case.GetFields() |> Seq.toList

          match caseFields with
          | [] -> $"export var default{name}_Case_{case.Name} = {{ Case: \"{case.Name}\" }}"
          | [ singleField ] ->
            let singleFieldCaseSignature = getFieldCaseName name case

            let singleFieldUnionCaseDefaultValue =
              renderSingleFieldUnionCaseDefaultValue callingModule case singleField

            if outerDu.IsGenericType then
              $"""export var default{singleFieldCaseSignature} = {anonymousFunctionSignature} => {singleFieldUnionCaseDefaultValue}"""
            else
              $"""export var default{singleFieldCaseSignature} = {singleFieldUnionCaseDefaultValue}"""
          | fields ->
            let signature = getFieldCaseName name case

            let multiFieldUnionCaseDefaultValue =
              renderMultiFieldUnionCaseDefaultValue callingModule case fields

            if outerDu.IsGenericType then
              $"""export var default{signature} = {anonymousFunctionSignature} => {multiFieldUnionCaseDefaultValue}"""
            else
              $"""export var default{signature} = {multiFieldUnionCaseDefaultValue}"""

        cases
        |> List.map renderCase
        |> String.concat System.Environment.NewLine *)

    let firstCaseName =
      match cases with
      | [] -> failwith "todo"
      | [ case ] ->
        let singleFieldCaseSignature = getFieldCaseName name case
        $"""default{singleFieldCaseSignature}"""
      | cases ->
        let case = cases.Head
        let singleFieldCaseSignature = getFieldCaseName name case
        $"""default{singleFieldCaseSignature}"""

    let renderCaseSignature (case: UnionCaseInfo) =
      let caseFields = case.GetFields() |> Seq.toList

      match caseFields with
      | [] -> $"""{name}_Case_{case.Name}"""
      | [ singleField ] ->
        let singleFieldCaseSignature = getSingleFieldCaseSignature name singleField case
        singleFieldCaseSignature
      | fields ->
        let singleFieldCaseSignature = getMultiFieldCaseSignature outerDu name fields case
        singleFieldCaseSignature

    let caseSignatures =
      cases
      |> List.map renderCaseSignature
      |> String.concat " | "

    let callParameters = genericArgumentListAsParametersCall outerDu

    let signature = getNamedFunctionSignatureForDefaultValue outerDu
    let defaultCase = firstCaseName

    let renderedDefaultCase =
      if outerDu.IsGenericType then
        $"""export var default{name} = {anonymousFunctionSignature} => {defaultCase}{callParameters} as {signature}"""
      else
        $"""export var default{name} = {defaultCase} as {signature}"""

    let definition =
      $"""{renderedCaseDefinitions}
export type {signature} = {caseSignatures}
export type {name}_Case = {caseNameLiteral}"""

    (*let value =
      $"""export var {name}_AllCases = [ {allCaseNames} ] as const
{renderedCaseDefaultNamesAndValues}
{renderedDefaultCase}"""*)

    renderDefinitionAndOrValue definition //value strategy

let ignoreList = [ typedefof<FSharpFunc<_, _>> ]

let isIgnored (x: System.Type) =
    let t =
      if x.IsGenericTypeDefinition then
        x
      else if x.IsGenericType then
        x.GetGenericTypeDefinition()
      else
        x

    let shouldBeIgnored = ignoreList |> List.exists (fun v -> v = t)
    shouldBeIgnored

let getProperties (t: System.Type) =
    // maybe ignore properties that have a NotMappedAttribute
    t.GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
    |> Seq.filter (fun x -> not (isIgnored x.PropertyType))

let renderRecord (t: System.Type) (strategy: RenderStrategy)(defaultTypes:PredefinedTypes.PreDefinedTypes) =
    let isAnonymous = isAnonymousRecord t

    if t.IsGenericType
       && not t.IsGenericTypeDefinition
       && not isAnonymous then
      failwith "A definition and value for a generic type that is not a generic type definition cannot be rendered"

    let callingModule = getModuleName t
    let name = getName t
    let properties = getProperties t

    let fields =
      properties
      |> Seq.map (fun x->renderPropertyNameAndDefinition callingModule x defaultTypes)
      |> String.concat System.Environment.NewLine

    let genericArguments = genericArgumentList t

    // let fieldValues =
    //   properties
    //   |> Seq.map (renderPropertyNameAndValue true callingModule)
    //   |> String.concat $",{System.Environment.NewLine}"

    let anonymousFunctionSignature = getAnonymousFunctionSignatureForDefaultValue t
    let namedFunctionSignature = getNamedFunctionSignatureForDefaultValue t

    //     let value =
//       if t.IsGenericTypeDefinition then
//         $"""export var default{name}: {anonymousFunctionSignature} => {namedFunctionSignature} = {anonymousFunctionSignature} => ({{
//   {fieldValues}
// }})"""
//       else
//         $"""export var default{name}: {name} = {{
//   {fieldValues}
// }}"""

    let definition =
      $"""export type {name}{genericArguments} = {{
  {fields}
}}
"""

    renderDefinitionAndOrValue definition // value strategy

let renderPredefinedType (t: System.Type) (predefined: PredefinedTypes.PredefinedValues) (strategy: RenderStrategy) =
    let name = predefined.Name |>Option.defaultValue( getName t)

    let definition, value =
      if t.IsGenericType then
        let genericArguments = genericArgumentList t
        let anonymousFunctionSignature = getAnonymousFunctionSignatureForDefaultValue t

        let definition =
          $"""export type {name}{genericArguments} = {(predefined.Definition
                                                       |> Option.defaultValue "unknown // renderPredefinedTypeFromDefaultValue (generic)")}"""

        let value =
          $"""export var default{name}: {anonymousFunctionSignature} => {name}{genericArguments} = {anonymousFunctionSignature} => {predefined.InlineDefaultValue
                                                                                                                                    |> Option.defaultValue "unknown"}"""

        definition, value
      else
        let definition =
          $"""export type {name} = {(predefined.Definition |> Option.defaultValue "any")}"""

        let value =
          $"""export var default{name}: {name} = {predefined.InlineDefaultValue
                                                  |> Option.defaultValue "unknown // renderPredefinedTypeFromDefaultValue (not generic)"}"""

        definition, value

    renderDefinitionAndOrValue definition //value strategy

let renderStubValue (t: System.Type) =
    let name = getName t
    $"""export var default{name}: {name} = {{ }} as any as {name} // this is a placeholder and is set again at the end of the file"""
