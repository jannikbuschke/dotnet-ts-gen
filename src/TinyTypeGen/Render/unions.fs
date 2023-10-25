module TinyTypeGen.Render.Unions

open System
open System.Reflection
open System.Text.Json.Serialization.TypeCache
open System.Text.Json.Serialization
open type JsonUnionEncoding
open Microsoft.FSharp.Reflection
open TinyTypeGen

type GetPropSignature = Type -> string
type Deps = Env * GetPropSignature * string array * (Type -> bool) * string

let (|Adjacent|_|) (encoding: JsonUnionEncoding) =
  if encoding.HasFlag AdjacentTag then Some Adjacent else None

let (|Internal|_|) (encoding: JsonUnionEncoding) =
  if encoding.HasFlag InternalTag then Some Internal else None

let (|External|_|) (encoding: JsonUnionEncoding) =
  if encoding.HasFlag ExternalTag then Some External else None

let (|UnwrapFieldless|_|) (encoding: JsonUnionEncoding) =
  if encoding.HasFlag JsonUnionEncoding.UnwrapFieldlessTags then
    Some UnwrapFieldless
  else
    None

let (|UnwrapSingleCaseUnion|_|) (encoding: JsonUnionEncoding) =
  if encoding.HasFlag UnwrapSingleCaseUnions then
    Some UnwrapSingleCaseUnion
  else
    None

let (|UnwrapRecordCase|_|) (encoding: JsonUnionEncoding) =
  if encoding.HasFlag UnwrapRecordCases then
    Some UnwrapRecordCase
  else
    None
let (|UnwrapSingleFieldCase|_|) (encoding: JsonUnionEncoding) =
  if encoding.HasFlag UnwrapSingleFieldCases then
    Some UnwrapSingleFieldCase
  else
    None

let (|NamedFields|_|) (encoding: JsonUnionEncoding) =
  if
    encoding.HasFlag JsonUnionEncoding.NamedFields
    || encoding.HasFlag UnwrapRecordCases
  then
    Some NamedFields
  else
    None

type Du =
  {
    Name: string
    Cases:
      {|
        Name: string
        Fields: PropertyInfo array
      |} array
  }

  static member From(t: Type) =
    {
      Name = t.Name
      Cases =
        t
        |> FSharpType.GetUnionCases
        |> Seq.map (fun v ->
          {|
            Name = v.Name
            Fields = v.GetFields()
          |}
        )
        |> Seq.toArray
    }

let getFieldCaseName (name: string) (case: UnionCaseInfo) = $"{name}_Case_{case.Name}"

let getSingleFieldCaseSignature (outerDu: Type) (name: string) (case: UnionCaseInfo) =
  let name = getFieldCaseName name case
  let genericParameterPostfix = genericArgumentList outerDu
  $"{name}{genericParameterPostfix}"

type Union = Type * UnionCaseInfo list

let namedField (field: PropertyInfo) (_: Env, getPropSignature: GetPropSignature) =
  sprintf "%s: %s" (field.Name |> Utils.toLower) (getPropSignature field.PropertyType)

let arrayField (field: PropertyInfo) (_: Env, getPropSignature: GetPropSignature) =
  (getPropSignature field.PropertyType)

let getNamedFields (fields: PropertyInfo list) (_: Env, getPropSignature: GetPropSignature) =
  fields
  |> List.map (fun field -> sprintf "%s: %s" (field.Name |> Utils.toLower) (getPropSignature field.PropertyType))
  |> String.concat ", "

let getArrayFields (fields: PropertyInfo list) (_: Env, getPropSignature: GetPropSignature) =
  fields
  |> List.map (fun field -> getPropSignature field.PropertyType)
  |> String.concat ", "

let renderFieldlessCaseDefinition (env: Env) =
  let encoding = env.Encoding
  match encoding with
  | External & NamedFields -> "{}"
  | External -> "[ ]"
  | _ -> ""

let renderMultiCaseSingleField (singleField: PropertyInfo) (deps: Deps) =
  let env, getPropSignature, _, isRecord, _ = deps
  match env.Encoding with
  | Adjacent & UnwrapSingleFieldCase -> singleField.PropertyType |> getPropSignature |> sprintf "Fields: %s"
  | Adjacent & UnwrapRecordCase when isRecord singleField.PropertyType ->
    singleField.PropertyType |> getPropSignature |> sprintf "Fields: %s"
  | (External | Internal) & UnwrapRecordCase when isRecord singleField.PropertyType ->
    singleField.PropertyType |> getPropSignature
  | External & UnwrapSingleFieldCase -> getPropSignature singleField.PropertyType
  | Adjacent & NamedFields ->
    getNamedFields [ singleField ] (env, getPropSignature)
    |> sprintf "Fields: { %s }"
  | Adjacent -> arrayField singleField (env, getPropSignature) |> sprintf "Fields: [ %s ]"
  | Internal & NamedFields -> getNamedFields [ singleField ] (env, getPropSignature)
  | Internal -> getArrayFields [ singleField ] (env, getPropSignature)
  | External & NamedFields -> getNamedFields [ singleField ] (env, getPropSignature) |> sprintf "{ %s }"
  | External -> getArrayFields [ singleField ] (env, getPropSignature) |> sprintf "[ %s ]"
  | _ -> failwith "not supported"

let renderSingleCaseSingleField (singleField: PropertyInfo) (deps: Deps) =
  let env, getPropSignature, _, isRecord, _ = deps
  match env.Encoding with
  | UnwrapSingleCaseUnion -> getPropSignature singleField.PropertyType
  | Adjacent & UnwrapSingleCaseUnion -> getPropSignature singleField.PropertyType |> sprintf "Fields: [ %s ]"
  | Adjacent & UnwrapRecordCase when isRecord singleField.PropertyType ->
    singleField.PropertyType |> getPropSignature |> sprintf "Fields: %s"
  | Internal & UnwrapRecordCase when isRecord singleField.PropertyType -> singleField.PropertyType |> getPropSignature
  | (External | Internal) & UnwrapRecordCase when isRecord singleField.PropertyType ->
    singleField.PropertyType |> getPropSignature
  | Adjacent & UnwrapSingleFieldCase -> singleField.PropertyType |> getPropSignature |> sprintf "Fields: %s"
  | External & UnwrapSingleCaseUnion -> getPropSignature singleField.PropertyType
  | External & UnwrapSingleFieldCase -> getPropSignature singleField.PropertyType
  | Internal & UnwrapSingleCaseUnion -> getPropSignature singleField.PropertyType
  | Adjacent & NamedFields ->
    getNamedFields [ singleField ] (env, getPropSignature)
    |> sprintf "Fields: { %s }"
  | Adjacent -> arrayField singleField (env, getPropSignature) |> sprintf "Fields: [ %s ]"
  | Internal & NamedFields -> getNamedFields [ singleField ] (env, getPropSignature)
  | Internal -> getArrayFields [ singleField ] (env, getPropSignature)
  | External & NamedFields -> getNamedFields [ singleField ] (env, getPropSignature) |> sprintf "{ %s }"
  | External -> getArrayFields [ singleField ] (env, getPropSignature) |> sprintf "[ %s ]"
  | _ -> failwith "not supported"

let fullyQualified tuple = sprintf "%s.%s" (fst tuple) (snd tuple)

let renderField (deps: Deps, getPropSignature: GetPropSignature) (field: PropertyInfo) =
  let env, _, _, isRecord, _ = deps
  match env.Encoding with
  // we are in multi field case
  | (Adjacent | External | Internal) & NamedFields -> namedField field (env, getPropSignature)
  | Adjacent
  | External when isRecord field.PropertyType -> field.PropertyType |> getPropSignature
  | Adjacent -> arrayField field (env, getPropSignature)
  | External & NamedFields -> namedField field (env, getPropSignature)
  | External -> arrayField field (env, getPropSignature)
  | Internal -> arrayField field (env, getPropSignature)
  | _ -> failwith "not supported"

let renderMultiFieldCase (fields: PropertyInfo list) (deps: Deps) =
  let env, getPropSignature, _, _, _ = deps
  fields
  |> List.map (renderField (deps, getPropSignature))
  |> String.concat ", "
  |> match env.Encoding with
     | Adjacent & NamedFields -> sprintf "Fields: { %s }"
     | External & NamedFields -> sprintf "{ %s }"
     | Adjacent -> sprintf "Fields: [ %s ]"
     | External -> sprintf "[ %s ]"
     | Internal -> sprintf "%s"
     | _ -> failwith "not supported"

let renderSingleCaseUnionCaseDefinition (singleCase: UnionCaseInfo) (deps: Deps) =
  let env, _, _, isRecord, _ = deps
  let fields = singleCase.GetFields() |> Seq.toList
  match fields with
  | [] -> renderFieldlessCaseDefinition env
  | [ singleField ] -> renderSingleCaseSingleField singleField deps
  | fields -> renderMultiFieldCase fields deps
  |> if fields.Length = 1 && env.Encoding.HasFlag UnwrapSingleCaseUnions then
       sprintf "%s"
     else

       let isSingleRecord = (fields.Length = 1 && isRecord (fields[0].PropertyType))
       match env.Encoding with
       | Adjacent & (UnwrapSingleCaseUnion | UnwrapFieldless) -> sprintf "{ Case: \"%s\", %s }" singleCase.Name
       | Adjacent -> sprintf "{ Case: \"%s\", %s }" singleCase.Name
       | Internal & UnwrapRecordCase when isSingleRecord -> sprintf "{ Case: \"%s\" } & %s" singleCase.Name
       | Internal & NamedFields -> sprintf "{ Case: \"%s\", %s }" singleCase.Name
       | Internal -> sprintf "[ \"%s\", %s ]" singleCase.Name
       | External -> sprintf "{ %s: %s }" singleCase.Name
       | _ -> failwith "not supported"

let renderMultiCaseUnionCaseDefinition (unionCase: UnionCaseInfo) (deps: Deps) =
  let env, _, _, isRecord, _ = deps
  let fields = unionCase.GetFields() |> Seq.toList
  match fields with
  | [] ->
    renderFieldlessCaseDefinition env
    |> match env.Encoding with
       | UnwrapFieldless -> fun _ -> sprintf "\"%s\"" unionCase.Name
       | Adjacent -> sprintf "{ Case: \"%s\", %s }" unionCase.Name
       | Internal & NamedFields -> sprintf "{ Case: \"%s\", %s }" unionCase.Name
       | Internal -> sprintf "[ \"%s\", %s ]" unionCase.Name
       | External -> sprintf "{ %s: %s }" unionCase.Name
       | _ -> failwith "not supported"
  | [ singleField ] ->
    renderMultiCaseSingleField singleField deps
    |> match env.Encoding with
       | Adjacent -> sprintf "{ Case: \"%s\", %s }" unionCase.Name
       | Internal & UnwrapRecordCase when isRecord singleField.PropertyType ->
         sprintf "{ Case: \"%s\" } & %s" unionCase.Name
       | Internal & NamedFields -> sprintf "{ Case: \"%s\", %s }" unionCase.Name
       | Internal -> sprintf "[ \"%s\", %s ]" unionCase.Name
       | External -> sprintf "{ %s: %s }" unionCase.Name
       | _ -> failwith "not supported"
  | fields ->
    renderMultiFieldCase fields deps
    |> match env.Encoding with
       | Adjacent -> sprintf "{ Case: \"%s\", %s }" unionCase.Name
       | Internal & NamedFields -> sprintf "{ Case: \"%s\", %s }" unionCase.Name
       | Internal -> sprintf "[ \"%s\", %s ]" unionCase.Name
       | External -> sprintf "{ %s: %s }" unionCase.Name
       | _ -> failwith "not supported"

let renderCaseDefinitions (t: Type, cases) (deps: Deps) =
  let env, _, _, _, extendedName = deps
  let genericArguments = genericArgumentList t
  let _, name = env.GetSignature t

  match cases with
  | [] ->
    failwith "not yet implemented"
    ""
  | [ singleCase ] ->
    renderSingleCaseUnionCaseDefinition singleCase deps
    |> sprintf "export type %s_Case_%s%s%s = %s" name singleCase.Name extendedName genericArguments
  | cases ->
    cases
    |> List.map (fun case ->
      sprintf
        "export type %s_Case_%s%s%s = %s"
        name
        case.Name
        extendedName
        genericArguments
        (renderMultiCaseUnionCaseDefinition case deps)
    )
    |> String.concat Environment.NewLine

let renderDuCore
  (outerDu: Type)
  (env: Env)
  (genericArgNames: string array)
  (recordExtendedName: string)
  (isRecordImpl: Type -> bool)
  (comment: string)
  =
  let callingModule = getModuleName outerDu
  let name = getName outerDu env.PreDefinedTypes
  let cases = FSharpType.GetUnionCases outerDu |> Seq.toList
  let union = outerDu, cases
  let getPropSig = getPropertySignature2 callingModule env
  let deps = env, getPropSig, genericArgNames, isRecordImpl, recordExtendedName
  let allCases = cases |> List.map (fun v -> $"\"{v.Name}\"") |> String.concat ",\n  "

  let caseNameLiteral =
    cases |> List.map (fun v -> $"\"{v.Name}\"") |> String.concat " | "

  let renderedCaseDefinitions = renderCaseDefinitions union deps

  let renderCaseSignature (case: UnionCaseInfo) =
    let genericParameterPostfix = genericArgumentList outerDu
    $"{name}_Case_{case.Name}{recordExtendedName}{genericParameterPostfix}"

  let caseSignaturesList =
    cases
    |> List.map (fun c ->
      {|
        name = c.Name
        fields = c.GetFields() |> Seq.toList
        signature = renderCaseSignature c
      |}
    )

  let caseSignatures =
    caseSignaturesList |> List.map _.signature |> String.concat " | "

  let genericArguments = genericArgumentList outerDu
  let genericArgumentNamesStr = genericArgumentNames outerDu |> String.concat ","
  let finalName = name + recordExtendedName
  let signature = finalName + genericArguments

  let model =
    {|
      name = finalName
      allCases = allCases
      cases = cases |> Seq.toArray
      du = outerDu |> Du.From
      signature = signature
      case_signatures = caseSignaturesList
      genericArguments = genericArguments
      genericArgumentNames = genericArgumentNamesStr
    |}

  let custom =
    try
      TinyTypeGen.Scriban.duTemplate
      |> Option.map _.Render(model)
      |> Option.defaultValue ""
    with ex ->
      "// Error rendering custom template: " + ex.Message

  comment
  + renderedCaseDefinitions
  + "
export type "
  + signature
  + " = "
  + caseSignatures
  + "
export type "
  + finalName
  + "_Case = "
  + caseNameLiteral
  + "
export const "
  + finalName
  + "_AllCases = [
  "
  + allCases
  + @"] satisfies "
  + finalName
  + "_Case[]

export function is"
  + finalName
  + "_Case(value: any): value is "
  + finalName
  + "_Case {
  return "
  + finalName
  + "_AllCases.includes(value)
}
"
  + custom

let renderGenericDu (outerDu: Type) (env: Env) (genericArgumentName: string array) =
  let genericArgNames2 = outerDu.GetGenericArguments() |> Array.map _.Name
  let recordExtendedName = "_" + (genericArgumentName |> String.concat "")
  let isRecordImpl (t: Type) =
    isRecord t || (genericArgumentName |> Array.contains t.Name)
  let comment =
    "// GENERIC RECORD DU: " + (genericArgumentName |> String.concat ",") + "\n"
  renderDuCore outerDu env genericArgNames2 recordExtendedName isRecordImpl comment

let renderDefaultDu (outerDu: Type) (env: Env) =
  let isRecordImpl (t: Type) = isRecord t
  renderDuCore outerDu env [||] "" isRecordImpl ""

let renderDu (outerDu: Type) (env: Env) =
  let result = renderDefaultDu outerDu env
  if
    env.Encoding.HasFlag JsonUnionEncoding.UnwrapRecordCases
    && outerDu.IsGenericType
  then
    let names = outerDu.GetGenericArguments() |> Array.map _.Name |> Seq.toList
    let powerSet (xs: 'a list) =
      List.fold (fun acc x -> acc @ (acc |> List.map (fun subset -> subset @ [ x ]))) [ [] ] xs
    let allSubsets = powerSet names |> List.filter (fun x -> x.Length > 0)

    allSubsets
    |> List.map (fun x -> renderGenericDu outerDu env (x |> Seq.toArray))
    |> String.concat Environment.NewLine
    |> fun genericRecordVariants ->
        $"""{result}
{genericRecordVariants}"""
  else
    result
