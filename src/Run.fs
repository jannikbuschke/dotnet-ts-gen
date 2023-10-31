module TsGen.Run

open System
open System.Text
open TsGen.Gen

let init (defaultTypes: PredefinedTypes.PreDefinedTypes) (types: Type list) (endpoints: ApiEndpoint list) =

  let collect = Collect.init defaultTypes
  let render = Gen.init defaultTypes

  let modules = collect.collectModules types

  let renderModule (m: TsModule) =

    let deps = collect.GetModuleDependencies m

    let builder = StringBuilder()

    builder
      .AppendLine("//////////////////////////////////////")
      .AppendLine("//   This file is auto generated   //")
      .AppendLine("//////////////////////////////////////")
      .AppendLine("")
    |> ignore

    deps
    |> List.iter (fun v -> builder.AppendLine($"import * as {v} from \"./{v}\"") |> ignore)

    builder.AppendLine() |> ignore

    let sorted, cyclics =
      TopologicalSort.topologicalSort
        (fun v ->
          let deps = collect.getDependencies v

          // Nullable remove
          deps |> List.filter (fun x -> x.Namespace = v.Namespace))

        m.Types

    if cyclics.Length > 0 then
      builder
        .AppendLine("//*** Cyclic dependencies dected ***")
        .AppendLine("//*** this can cause problems when generating types and defualt values ***")
        .AppendLine("//*** Please ensure that your types don't have cyclic dependencies ***")
      |> ignore

      cyclics |> List.iter (fun v -> builder.AppendLine("// " + v.Name) |> ignore)

      builder
        .AppendLine("//*** ******************* ***")
        .AppendLine("")
      |> ignore


    sorted
    |> List.distinct
    |> List.map (fun v ->

      let isCyclic = cyclics |> List.contains v

      let result =
        // workaround for Bullable<Guid>
        if v.IsGenericType && not v.IsGenericTypeDefinition && v.Name.Contains("Nullable") then
          ""
        else if isCyclic then
          $"""
  // This type has cyclic dependencies: {v.FullName}
  // in general this should be avoided. We render a 'stub' value here that will be changed at the bottom of this file
  {render.renderType v RenderStrategy.RenderDefinition}
  {render.renderStubValue v}
  """
        else
          render.renderType v RenderStrategy.RenderDefinitionAndValue

      result)

    |> List.map Utils.cleanTs
    |> List.iter (fun v -> builder.AppendLine(v) |> ignore)

    if cyclics.Length > 0 then
      builder.AppendLine("// Render cyclic fixes") |> ignore

    cyclics
    |> List.distinct
    |> List.map (fun v ->
      let name = getName v

      $"""//
  // the type {v.FullName} has cyclic dependencies
  // in general this should be avoided
  //
  Object.assign(default{name}, ({render.renderType v RenderStrategy.RenderValue}))
  """)
    |> List.map Utils.cleanTs
    |> List.iter (fun v -> builder.AppendLine(v) |> ignore)

    builder.ToString().Replace("\r\n", "\n")

  let renderModules path (modules: TsModule list) =

    if not (System.IO.Directory.Exists(path)) then
      System.IO.Directory.CreateDirectory path |> ignore
      ()

    // TODO remember old files
    System.IO.Directory.EnumerateFiles path
    |> Seq.iter (fun file -> System.IO.File.Delete(file))


    let distinctModules = modules |> List.distinctBy (fun v -> v.Name)

    if distinctModules.Length <> modules.Length then
      failwith "modules not distinct"

    distinctModules
    |> List.iter (fun v ->
      let fs = renderModule v
      let sanitizedName = v.Name

      let fileName = v.Name
      let filePath = $"{path}{fileName}.ts"

      System.IO.File.WriteAllText(filePath, fs)

      System.IO.File.AppendAllText($"{path}index.ts", sprintf "import * as %s from './%s'%s" sanitizedName sanitizedName Environment.NewLine)
      ())

    distinctModules
    |> List.iter (fun v ->
      let sanitizedName = v.Name

      System.IO.File.AppendAllText($"{path}index.ts", sprintf "export { %s }%s" sanitizedName Environment.NewLine)

      ())

    ()

  let renderTypes path =
    printfn "Generate TS types..."

    let stopWatch = System.Diagnostics.Stopwatch.StartNew()

    renderModules path modules

    stopWatch.Stop()
    printfn "Generated client types in %d ms" (stopWatch.Elapsed.TotalMilliseconds |> Math.Round |> Convert.ToInt32)
    ()

  let renderApi path =
    printfn "Generate TS api..."

    let stopWatch = System.Diagnostics.Stopwatch.StartNew()

    let appendTo (builder: StringBuilder) (value: string) = builder.AppendLine(value) |> ignore

    let api, input, output, method =
      System.Text.StringBuilder(), System.Text.StringBuilder(), System.Text.StringBuilder(), System.Text.StringBuilder()

    modules
    |> List.iter (fun m ->
      $"""import * as {m.Name} from "./{m.Name}" """ |> appendTo api
      ())

    "" |> appendTo api
    $"""export type Input = {{""" |> appendTo input
    $"""export type Output = {{""" |> appendTo output
    $"""export type Method = {{""" |> appendTo method

    endpoints
    |> Seq.groupBy (fun v -> v.Method)
    |> Seq.iter (fun (httpVerb, endpoints) ->
      $"""export type {httpVerb.ToString()} = {{""" |> appendTo api

      endpoints
      |> Seq.iter (fun endpoint ->
        let getTypename t = (t |> Gen.getPropertySignature "")

        let inputTypeName = endpoint.Request |> getTypename
        let outputTypename = endpoint.Response |> getTypename

        $"""  "{endpoint.Route}": {inputTypeName};""" |> appendTo input
        $"""  "{endpoint.Route}": {outputTypename};""" |> appendTo output
        $"""  "{endpoint.Route}": "{endpoint.Method.ToString()}";""" |> appendTo method
        $"""  "{endpoint.Route}": "{endpoint.Route}";""" |> appendTo api
        ())

      $"""}}""" |> appendTo api
      "" |> appendTo api

      ())

    $"""}}""" |> appendTo output
    $"""}}""" |> appendTo input
    $"""}}""" |> appendTo method

    api
      .Append(input.ToString())
      .AppendLine("")
      .Append(output.ToString())
      .AppendLine("")
      .Append(method.ToString())
    |> ignore

    System.IO.File.WriteAllText(path, api.ToString())
    stopWatch.Stop()
    printfn "Generated client api in %d ms" (stopWatch.Elapsed.TotalMilliseconds |> Math.Round |> Convert.ToInt32)

  {| renderTypes = renderTypes
     renderApi = renderApi |}
