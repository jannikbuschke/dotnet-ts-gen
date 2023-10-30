module Glow.Core.TsGen.Generate2

open System
open System.Diagnostics
open System.Reflection
open Glow.SecondApproach

let renderTypes2 path (modules: TsModule list) =

    printfn "Generate ts types"

    if not (System.IO.Directory.Exists(path)) then
        System.IO.Directory.CreateDirectory path |> ignore
        ()

    // TODO remember old files
    System.IO.Directory.EnumerateFiles path
    |> Seq.iter (fun file -> System.IO.File.Delete(file))

    let stopWatch = System.Diagnostics.Stopwatch.StartNew()

    let distinctModules = modules |> List.distinctBy (fun v -> v.Name)

    if distinctModules.Length <> modules.Length then
        failwith "modules not distinct"

    distinctModules
    |> List.iter (fun v ->
        let fs = Glow.SecondApproach.renderModule v
        let sanitizedName = v.Name

        let fileName = v.Name
        let filePath = $"{path}{fileName}.ts"

        System.IO.File.WriteAllText(filePath, fs)

        System.IO.File.AppendAllText(
            $"{path}index.ts",
            sprintf "import * as %s from './%s'%s" sanitizedName sanitizedName Environment.NewLine
        )

        ())

    distinctModules
    |> List.iter (fun v ->
        let sanitizedName = v.Name

        System.IO.File.AppendAllText($"{path}index.ts", sprintf "export { %s }%s" sanitizedName Environment.NewLine)

        ())

    stopWatch.Stop()
    printfn "Generated time in %f ms" stopWatch.Elapsed.TotalMilliseconds
    ()

let renderTypes path (types: System.Type list) =

    let allTypes =
        types
        @ [ typedefof<System.Collections.Generic.List<_>>
            typedefof<System.Guid>
            typedefof<System.Boolean>
            typedefof<System.Int16>
            typedefof<System.Int32>
            typedefof<System.Int64>
            typedefof<System.Int128>
            typedefof<System.Byte>
            typedefof<System.Char>
            typedefof<System.String>
            typedefof<System.UInt16>
            typedefof<System.UInt32>
            typedefof<System.UInt64>
            typedefof<System.Decimal>
            typedefof<System.DateTime>
            typedefof<System.DateTimeOffset>
            typedefof<System.TimeSpan>
            typedefof<System.TimeSpan>
            typedefof<System.Object> ]
          @ (types |> List.collect getDependencies)

    let x =
        allTypes
        |> List.distinct
        |> List.groupBy getModuleName
        |> List.map (fun (v, items) -> { Name = v; Types = items })

    renderTypes2 path x

    ()
