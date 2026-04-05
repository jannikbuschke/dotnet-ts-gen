module TinyTypeGen.RenderApiWithScriban

open System
open System.Text
open Scriban
open TinyTypGen.Artifact
open TinyTypeGen.Config
open TinyTypeGen.ResourceHelper

// type ApiLayout =
//   | Flat of
//     {|
//       EmbedMethod: bool
//       StringPorperty: bool
//     |}
//   | Hierarchical

type Tree<'a> = | Node of string * int * Tree<'a> list * 'a list

let rec insert (path: string list) (v: 'a) (Node(name, depth, children, values): Tree<'a>) : Tree<'a> =
  match path with
  | [] -> Node(name, depth, children, v :: values)
  | head :: rest ->
    let matched, others =
      children
      |> List.partition (
        function
        | Node(nm, _, _, _) -> nm = head
      )

    match matched with
    | [] ->
      let newChild = insert rest v (Node(head, depth + 1, [], []))
      Node(name, depth, newChild :: children, values)
    | Node(nm, childdepth, cchildren, cvalues) :: _ ->
      let updatedChild = insert rest v (Node(nm, childdepth, cchildren, cvalues))
      let newChildren = updatedChild :: others
      Node(name, depth, newChildren, values)

let buildTree (rootName: string) (pairs: (string list * 'a) list) : Tree<'a> =
  List.fold (fun acc (path, value) -> insert path value acc) (Node(rootName, 0, [], [])) pairs

open System.Linq

let rec iter fNode fNode2 fValue (Node(name, depth, children, values): Tree<'a>) =
  fNode name values depth
  values |> List.iter (fValue name depth)
  children |> List.iter (iter fNode fNode2 fValue)
  fNode2 name values depth

let renderApiToString (apiTemplate: Scriban.Template) endpoints (env: Env) (modules: TsModuleWithDeps list) =

  let stopWatch = Diagnostics.Stopwatch.StartNew()

  let appendTo (builder: StringBuilder) (value: string) = builder.AppendLine value |> ignore

  let imports = StringBuilder()

  modules
  |> List.iter (fun m -> sprintf "import * as %s from \"./%s\"" m.Name m.Name |> appendTo imports)

  let api = StringBuilder()

  let renderEndpoints rootName endpoints =
    let tree =
      buildTree
        "r"
        (endpoints
         |> Seq.map (fun ep ->
           let path =
             ep.Route.Replace("-", "_").Split("/").Where(fun v -> v <> "") |> Seq.toList
           path, (ep, path.Length)
         )
         |> Seq.toList)

    // let padLeft amount (s: string) = s.PadLeft(s.Length + (amount * 2))

    let getTypename t =
      getPropertySignature null t env.GetSignature env.GetName
    if endpoints |> List.isEmpty then
      ()
    else
      endpoints
      |> List.groupBy _.Kind
      |> List.iter (fun (kind, endpoints) ->
        sprintf "export type %O = {" kind |> appendTo api

        endpoints
        |> List.groupBy _.Route
        |> List.iter (fun (route, endpoints) ->
          sprintf "    \"%s\": { " route |> appendTo api
          endpoints
          |> List.iter (fun e ->
            match e.Error with
            | [] ->
              sprintf
                "              %s: [%s, %s],"
                (e.Method.ToString())
                (e.Request |> getTypename)
                (e.Response |> getTypename)
              |> appendTo api
            | errors ->
              let errors = errors |> List.map (fst >> getTypename) |> String.concat " | "
              sprintf
                "              %s: [%s, %s, %s],"
                (e.Method.ToString())
                (e.Request |> getTypename)
                (e.Response |> getTypename)
                errors
              |> appendTo api
          )
          "    }," |> appendTo api
        )

        "}" |> appendTo api
      )
  // sprintf "  }," |> appendTo api

  // "export type Api2 = {" |> appendTo api
  // tree
  // |> iter
  //   (fun path x d ->
  //     // sprintf "path %s" path |> appendTo api
  //     if d = 0 then
  //       sprintf """//export const %s = { """ rootName |> appendTo api
  //     else if x = [] then
  //       (sprintf "%s: {" path) |> padLeft (d + 1) |> appendTo api
  //     else
  //       (sprintf "%s: {" path) |> padLeft (d + 1) |> appendTo api
  //   )
  //   (fun path x d ->
  //     let d = if d < 0 then 0 else d
  //
  //     if x = [] then
  //       (sprintf "}%s" (if d = 0 then "" else ",")) |> padLeft d |> appendTo api
  //     else
  //
  //       (sprintf "}%s" (if d = 0 then "" else ",")) |> padLeft d |> appendTo api
  //   )
  //
  //   (fun path d (endpoint, d2) ->
  //     let scriptObject1 = ScriptObject()
  //     // Notice: MyObject is not imported but accessible through
  //     // the variable myobject
  //
  //     let inputTypeName = endpoint.Request |> getTypename
  //     let outputTypename = endpoint.Response |> getTypename
  //     let outputNullableSuffix = if endpoint.ResponseNullable then " | null" else ""
  //     let output = outputTypename + outputNullableSuffix
  //     let prefix = if endpoint.Method = HttpVerb.POST then "post" else "get"
  //     scriptObject1["endpoint"] <- endpoint
  //     scriptObject1["path"] <- path
  //     scriptObject1["input"] <- inputTypeName
  //     scriptObject1["output"] <- output
  //     scriptObject1["method"] <- prefix
  //     scriptObject1.Import("fn", Func<Type, string>(getTypename))
  //     scriptObject1.Import("getTypeName", Func<Type, string>(getTypename))
  //
  //     let context = TemplateContext()
  //     context.PushGlobal(scriptObject1)
  //     let d2 = if d2 < 0 then 0 else d2
  //     let result = endpointTemplate.Render(context) |> padLeft d2
  //     api.AppendLine result |> ignore
  //
  //   )

  // "}" |> appendTo api

  // tree
  // |> iter
  //   (fun path x d ->
  //     if d = 0 then
  //       sprintf """export const %s = { """ rootName |> appendTo api
  //     else if x = [] then
  //       (sprintf "%s: {" path) |> padLeft d |> appendTo api
  //     else
  //       (sprintf "%s: {" path) |> padLeft d |> appendTo api
  //   )
  //   (fun path x d ->
  //     let d = if d < 0 then 0 else d
  //
  //     if x = [] then
  //       (sprintf "}%s" (if d = 0 then "" else ",")) |> padLeft d |> appendTo api
  //     else
  //
  //       (sprintf "}%s" (if d = 0 then "" else ",")) |> padLeft d |> appendTo api
  //   )
  //
  //   (fun path d (endpoint, d2) ->
  //     let scriptObject1 = ScriptObject()
  //     // Notice: MyObject is not imported but accessible through
  //     // the variable myobject
  //     let getTypename t =
  //       getPropertySignature null t env.GetSignature env.GetName
  //
  //     let inputTypeName = endpoint.Request |> getTypename
  //     let outputTypename = endpoint.Response |> getTypename
  //     let outputNullableSuffix = if endpoint.ResponseNullable then " | null" else ""
  //     let output = outputTypename + outputNullableSuffix
  //     let prefix = if endpoint.Method = HttpVerb.POST then "post" else "get"
  //     scriptObject1["endpoint"] <- endpoint
  //     scriptObject1["path"] <- path
  //     scriptObject1["input"] <- inputTypeName
  //     scriptObject1["output"] <- output
  //     scriptObject1["method"] <- prefix
  //     scriptObject1.Import("fn", Func<Type, string>(getTypename))
  //     scriptObject1.Import("getTypeName", Func<Type, string>(getTypename))
  //
  //     let context = TemplateContext()
  //     context.PushGlobal(scriptObject1)
  //     let d2 = if d2 < 0 then 0 else d2
  //     let result = endpointTemplate.Render(context) |> padLeft d2
  //     api.AppendLine result |> ignore
  //
  //   )

  renderEndpoints "api" endpoints

  let result =
    apiTemplate.Render
      {|
        imports = imports.ToString()
        api = api.ToString()
      |}

  stopWatch.Stop()

  printfn
    "Generated client api based on scriban in %d ms"
    (stopWatch.Elapsed.TotalMilliseconds |> Math.Round |> Convert.ToInt32)

  result

let renderApiToString2 (env: Env) (modules: TsModuleWithDeps list) (api: Api) =

  let loadTemplate (template: Config.Template) (defaultResourceTemplate: string) =
    match template with
    | Template.Default -> ReadEmbeddedText defaultResourceTemplate
    | Template.EmbeddedTemplate embeddedTemplate ->
      match embeddedTemplate with
      | EmbeddedTemplate.SimpleApiTemplate -> ReadEmbeddedText defaultTemplates.api
      | EmbeddedTemplate.ApiTemplateWithTanstackQuery -> ReadEmbeddedText defaultTemplates.api_tanstack_query
    | Template.OfString s -> s
    | Template.OfFile s -> s |> IO.Path.GetFullPath |> System.IO.File.ReadAllText

  let apiTemplate = loadTemplate api.ApiTemplate defaultTemplates.api

  // let endpointTemplate = loadTemplate api.EndpointTemplate defaultTemplates.endpoint
  // endpointTemplate |> printfn "template: %s"
  // let endpointTemplate = endpointTemplate |> Template.Parse

  let apiTemplate = apiTemplate |> Template.Parse

  renderApiToString apiTemplate api.Endpoints env modules

let renderApiToArtifact (env: Env) (modules: TsModuleWithDeps list) (api: Api) =
  let path = api.TargetFile
  let name, path =
    if path.EndsWith ".ts" then
      IO.Path.GetFileName path, IO.Path.GetDirectoryName path
    else
      "api.ts", path

  let content = renderApiToString2 env modules api
  toArtifact name null content path
