module IntegrationTests.Gen

open System.Diagnostics
open System.IO
open FsCheck
open Program
open TinyTypeGen.Config
open Xunit
open Microsoft.AspNetCore.Mvc.Testing
open System.Text.Json.Serialization
open type JsonUnionEncoding
open FsCheck.FSharp
open System.Text.RegularExpressions
open IntegrationTests

let stripPropertyNameQuotes (input: string) : string =
  Regex.Replace(input, @"""(\w+)""\s*:", "$1:")

let runCli (exe: string) (args: string) =
  let path = Path.GetFullPath "../../../examples/"
  let psi =
    ProcessStartInfo(
      WorkingDirectory = path,
      FileName = exe,
      Arguments = args,
      UseShellExecute = false,
      RedirectStandardOutput = true,
      RedirectStandardError = true
    )
  use proc = new Process(StartInfo = psi)
  proc.Start() |> ignore
  proc.WaitForExit()
  if proc.ExitCode <> 0 then
    let error = proc.StandardError.ReadToEnd()
    Error error
  else
    let result = proc.StandardOutput.ReadToEnd().Trim()
    Ok result

let rnd = Rnd 0UL
let sample v = Gen.sampleWithSeed rnd 50 v
let defaultSample<'t> count =
  ArbMap.defaults.ArbFor<'t>().Generator |> sample count

[<Fact>]
let gen () =
  let optionInt = defaultSample<Option<int>> 2
  let optionOfAnonRecordValue = defaultSample<Option<{| Val: int |}>> 5
  let optionOfResult = defaultSample<Option<Result<int, bool>>> 5
  let optionOfRecord = defaultSample<Option<MyRecord>> 5
  let fsResult = defaultSample<Result<int, bool>> 3
  let fsResultT = defaultSample<Result<MyRecord, bool>> 3
  let fsResultTError = defaultSample<Result<bool, MyRecord>> 3
  let fsResultTTError = defaultSample<Result<MyRecord, MyRecord>> 3

  let enumLike = defaultSample<EnumLikeUnion> 5
  let genericDu0Primitive = defaultSample<GenericDu0<int>> 1
  let genericDu0Record = defaultSample<GenericDu0<MyRecord>> 1
  let genericDu = defaultSample<GenericDu<int, bool>> 15
  let multiCaseMultiFields = defaultSample<MultiCaseMultiFields> 5
  let singleCaseUnion = defaultSample<SingleCaseUnion> 1

  use csWebapp = new WebApplicationFactory<CSharpWebapp.Program>()
  let csharpEndpoints = TinyTypeGen.AspNetCore.getEndpoints csWebapp.Services

  use fsWebapp = new WebApplicationFactory<Program>()
  let fsharpEndpoints =
    TinyTypeGen.AspNetCore.getEndpointsBasedOnEndpointDataSource fsWebapp.Services

  let special =
    [
      Default, "__Default"
      ThothLike, "__ThothLike"
      FSharpLuLike, "__FSharpLuLike"
      NewtonsoftLike, "__NewtonsoftLike"
      TinyTypeGen.Config.defaultJsonUnionEncoding,
      "__custom_AdjacentTag_UnwrapSingleFieldCases_UnwrapRecordCases_UnwrapOption_UnwrapSingleCaseUnions_AllowUnorderedTag"
    ]

  let tags = [ AdjacentTag; InternalTag; ExternalTag ]
  let options =
    [
      NamedFields
      UnwrapFieldlessTags
      UnwrapOption
      UnwrapSingleCaseUnions
      UnwrapSingleFieldCases
      UnwrapRecordCases
    ]

  let powerSet (xs: 'a list) =
    List.fold (fun acc x -> acc @ (acc |> List.map (fun subset -> subset @ [ x ]))) [ [] ] xs
  let allOptionCombis = powerSet options
  let reallyAllOptions =
    tags
    |> List.collect (fun x -> allOptionCombis |> List.map (List.fold (|||) x))
    |> List.distinct
    |> List.map (fun encoding ->
      encoding,
      encoding
        .ToString()
        .Replace("SuccintOption", "UnwrapOption")
        .Replace("BareFieldlessTags", "UnwrapFieldlessTags")
        .Replace("EraseSingleCaseUnions", "UnwrapSingleCaseUnions")
        .Replace(",", "")
        .Replace(" ", "_")
    )

  let dynamicType = CSharpWebapp.Test.CreateDynamicType()
  special @ reallyAllOptions
  |> List.iter (fun (encoding, name) ->
    let path = $"../../../examples/_{name}/"
    if Directory.Exists path then
      Directory.Delete(path, true)
    TinyTypeGen
      .GeneratorBuilder()
      // set union encoding
      .WithEncoding(encoding)
      // add custom types
      .AddTypes(
        [|
          typedefof<Result<string, string>>
          typedefof<MultiCaseMultiFields>
          typedefof<SingleCaseUnion>
          typedefof<EnumLikeUnion>
          typedefof<GenericDu<_, _>>
          typedefof<GenericDu0<_>>
          typedefof<MyRecord>
          dynamicType
        |]
      )
      // add api
      .AddApi(
        {
          Endpoints = csharpEndpoints
          TargetFile = $"{path}/api-cs.ts"
          ApiTemplate = Template.EmbeddedTemplate EmbeddedTemplate.ApiTemplateWithTanstackQuery
          EndpointTemplate = Template.Default
        }
      )
      // add another api
      .AddApi(
        {
          Endpoints = fsharpEndpoints
          TargetFile = $"{path}/api-fs.ts"
          ApiTemplate = Template.Default
          EndpointTemplate = Template.Default
        }
      )
      .Build()
      .RenderAll
      path

    let serialize = fun x -> serializeWithEncoding encoding x |> stripPropertyNameQuotes
    let result = multiCaseMultiFields |> serialize
    let enumLike = enumLike |> serialize
    let singleCase = singleCaseUnion |> serialize
    let genericDu = genericDu |> serialize

    let optionInt = optionInt |> serialize
    let optionOfAnonRecordValue = optionOfAnonRecordValue |> serialize
    let optionOfResult = optionOfResult |> serialize
    let optionOfRecord = optionOfRecord |> serialize
    let fsResult = fsResult |> serialize
    let fsResultTTError = fsResultTTError |> serialize
    let fsResultTError = fsResultTError |> serialize
    let fsResultT = fsResultT |> serialize
    let genericDu0Primitive = genericDu0Primitive |> serialize
    let genericDu0Record = genericDu0Record |> serialize
    let unwrapRecordCases = encoding.HasFlag UnwrapRecordCases
    let importRecordVariant = if unwrapRecordCases then "GenericDu0_a," else ""
    let genericRecord =
      if unwrapRecordCases then
        $"""const generic0Record = {genericDu0Record} as const satisfies GenericDu0_a<MyRecord>[]"""
      else
        ""

    $"""
// {name}
import {{
  MultiCaseMultiFields,
  SingleCaseUnion,
  EnumLikeUnion,
  GenericDu,
  MyRecord,
  GenericDu0,
  {importRecordVariant}
  }} from "./IntegrationTests";
import {{
  FSharpResult,
  FSharpOption,
  {(if unwrapRecordCases then
      "FSharpResult_T, FSharpResult_TError, FSharpOption_T, FSharpResult_TTError"
    else
      "FSharpResult as FSharpResult_T, FSharpResult as FSharpResult_TTError, FSharpResult as FSharpResult_TError, FSharpOption as FSharpOption_T")}
}} from "./Microsoft_FSharp_Core"
import * as System from "./System"

const generic0Primitive = {genericDu0Primitive} as const satisfies GenericDu0<number>[]

{genericRecord}

const enumLike = {enumLike} as const satisfies EnumLikeUnion[]

const singleCase = {singleCase} as const satisfies SingleCaseUnion[]

const multiCase = {result} as const satisfies MultiCaseMultiFields[]

const generic = {genericDu} as const satisfies GenericDu<number,boolean>[]

const optionInt = {optionInt} as const satisfies FSharpOption<System.Int32>[]

const optionOfAnonRecordValue = {optionOfAnonRecordValue} as const satisfies FSharpOption_T<{{ val: number }}>[]

const optionOfResult = {optionOfResult} as const satisfies FSharpOption<FSharpResult<System.Int32,System.Boolean>>[]

const optionOfRecord = {optionOfRecord} as const satisfies FSharpOption_T<MyRecord>[]

const resultPrimitive = {fsResult} as const satisfies FSharpResult<System.Int32,System.Boolean>[]
const resultT = {fsResultT} as const satisfies FSharpResult_T<MyRecord,System.Boolean>[]
const resultTError = {fsResultTError} as const satisfies FSharpResult_TError<System.Boolean,MyRecord>[]
const resultTTError = {fsResultTTError} as const satisfies FSharpResult_TTError<MyRecord,MyRecord>[]

// {name}
"""
    |> fun content -> File.WriteAllText($"{path}json.ts", content)
  )
