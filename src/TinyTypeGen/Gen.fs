module TsGen.Gen

open System.Reflection
open System.Text.Json.Serialization
open TsGen
open Microsoft.FSharp.Reflection
open TypeCache

let defaultJsonUnionEncoding =
  JsonUnionEncoding.AdjacentTag
  ||| JsonUnionEncoding.UnwrapSingleFieldCases
  ||| JsonUnionEncoding.UnwrapRecordCases
  ||| JsonUnionEncoding.UnwrapOption
  ||| JsonUnionEncoding.UnwrapSingleCaseUnions
  ||| JsonUnionEncoding.AllowUnorderedTag

let supportedJsonUnionEncoding = defaultJsonUnionEncoding

let renderPropertyNameAndDefinition (callingModule: string) (fieldInfo: PropertyInfo) =
  let signature = getPropertySignature callingModule fieldInfo.PropertyType
  let name = fieldInfo.Name
  let fieldName = Utils.camelize name
  $"""  {fieldName}: {signature}"""

let init (defaultTypes: PredefinedTypes.PreDefinedTypes) (jsonUnionEncoding: JsonUnionEncoding) =
  let tryGetPredefinedType = PredefinedTypes.tryPredefinedType defaultTypes

  let renderSingleFieldUnionCaseDefinition (callingModule: string) (case: UnionCaseInfo) (fieldInfo: PropertyInfo) =
    $"""{{ Case: "{case.Name}", Fields: {getPropertySignature callingModule fieldInfo.PropertyType} }}"""

  let renderMultiFieldUnionCaseDefinition
    (callingModule: string)
    (outerDu: System.Type)
    (case: UnionCaseInfo)
    (fieldInfo: PropertyInfo list)
    =
    let fields =
      fieldInfo
      |> List.map (fun v ->
        (Utils.camelize v.Name)
        + ": "
        + getDuPropertySignature callingModule v.PropertyType)
      |> String.concat ", "

    $"""{{ Case: "{case.Name}", Fields: {{ {fields} }} }}"""

  let getDefaultValue (callingModule: string) (propertyType: System.Type) =
    let moduleName = getModuleName propertyType
    let name = getName propertyType
    let isGeneric = propertyType.IsGenericType

    let prefix =
      if moduleName = callingModule then
        ""
      else
        moduleName + "."

    let kind = getKind propertyType

    let predefinedValue =
      tryGetPredefinedType propertyType
      |> Option.map (fun v -> v.InlineDefaultValue)
      |> Option.flatten

    let postfix =
      if isGeneric then
        (getGenericParameterValues callingModule propertyType)
      else
        ""

    let value =
      predefinedValue
      |> Option.defaultValue (
        match kind with
        | TypeKind.Record -> $"""{prefix}default{name}{postfix}"""
        | TypeKind.List -> "[]"
        | TypeKind.Array -> "[]"
        | TypeKind.Map ->
          if propertyType.GenericTypeArguments.[0] = typeof<string> then
            "({})"
          else
            "[]"
        | TypeKind.Union -> $"""{prefix}default{name}{postfix}"""
        | _ -> $"""{prefix}default{name}{postfix}"""
      )

    value

  let renderPropertyNameAndValue (camelize: bool) (callingModule: string) (fieldInfo: PropertyInfo) =
    let propertyType = fieldInfo.PropertyType
    let value = getDefaultValue callingModule propertyType

    if camelize then
      $"""  {Utils.camelize fieldInfo.Name}: {value}"""
    else
      $"""  {fieldInfo.Name}: {value}"""

  let renderSingleFieldUnionCaseDefaultValue (callingModule: string) (case: UnionCaseInfo) (fieldInfo: PropertyInfo) =
    let defaultValue = getDefaultValue callingModule fieldInfo.PropertyType

    if fieldInfo.PropertyType.IsGenericParameter then
      $"""({{ Case: "{case.Name}", Fields: {defaultValue} }})"""
    else
      $"""{{ Case: "{case.Name}", Fields: {defaultValue} }}"""

  let renderMultiFieldUnionCaseDefaultValue
    (callingModule: string)
    (case: UnionCaseInfo)
    (fieldInfo: PropertyInfo list)
    =

    let isGeneric =
      fieldInfo
      |> List.exists (fun v -> v.PropertyType.IsGenericParameter)

    let fields =
      fieldInfo
      |> List.map (fun v -> renderPropertyNameAndValue false callingModule v)
      |> String.concat ", "

    if isGeneric then
      $"""({{ Case: "{case.Name}", Fields: {{ {fields} }})"""
    else
      $"""{{ Case: "{case.Name}", Fields: {{ {fields} }}  }}"""

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

  let renderDu (outerDu: System.Type) (strategy: RenderStrategy) =

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

          let prop = getPropertySignature callingModule singleField.PropertyType
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
              renderSingleFieldUnionCaseDefinition callingModule case singleField

            $"""export type {singleFieldCaseSignature} = {singleFieldUnionCaseDefinition}"""
          | fields ->
            let singleFieldCaseSignature = getMultiFieldCaseSignature outerDu name fields case

            let multiFieldUnionCaseDefinition =
              renderMultiFieldUnionCaseDefinition callingModule outerDu case fields

            $"""export type {singleFieldCaseSignature} = {multiFieldUnionCaseDefinition}"""

        cases
        |> List.map renderCase
        |> String.concat System.Environment.NewLine

    let anonymousFunctionSignature =
      getAnonymousFunctionSignatureForDefaultValue outerDu

    let renderedCaseDefaultNamesAndValues =
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
        |> String.concat System.Environment.NewLine

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

    let value =
      $"""export var {name}_AllCases = [ {allCaseNames} ] as const
{renderedCaseDefaultNamesAndValues}
{renderedDefaultCase}"""

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
    t.GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
    |> Seq.filter (fun x -> not (isIgnored x.PropertyType))

  let renderRecord (t: System.Type) (strategy: RenderStrategy) =
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
      |> Seq.map (renderPropertyNameAndDefinition callingModule)
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
    let name = getName t

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

  let renderType (t: System.Type) (strategy: RenderStrategy) =
    let kind = getKind t

    let predefinedDefinitionAndValue =
      tryGetPredefinedType t
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
export var {name}_AllValues = [{values |> String.concat ", "}] as const
export var default{name}: {name} = {values |> List.head}
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