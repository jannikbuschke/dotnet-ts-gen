module TinyTypeGen.ResourceHelper

open System
open System.IO
open System.Linq
open System.Reflection

let defaultTemplates =
  {|
    api = "template.api.scriban"
    api_tanstack_query = "template.api-tanstack-query.scriban"
    union = "template.union.scriban"
    endpoint = "template.endpoint.scriban"
    modul = "template.module.scriban"
  |}

let ReadEmbeddedText (resourceFileName: string) =
  let assembly = Assembly.GetExecutingAssembly()
  let resourcePath =
    assembly.GetManifestResourceNames().Single(_.EndsWith(resourceFileName, StringComparison.Ordinal))
  use stream = assembly.GetManifestResourceStream(resourcePath)
  use reader = new StreamReader(stream)
  reader.ReadToEnd()
