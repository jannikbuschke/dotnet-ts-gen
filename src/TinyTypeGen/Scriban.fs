module TinyTypeGen.Scriban

let tryReadPath path =
  if System.IO.File.Exists path then
    path |> System.IO.File.ReadAllText |> Some
  else
    None

let loadTemplate path =
  path
  |> System.IO.Path.GetFullPath
  |> tryReadPath
  |> fun template ->
      if template.IsNone then
        printfn "Warning: template not found at path %s" (path |> System.IO.Path.GetFullPath)

      template |> Option.map Scriban.Template.Parse

let duTemplate = loadTemplate "../client/template.du.scriban"
let moduleTemplate = loadTemplate "../client/template.module.scriban"
