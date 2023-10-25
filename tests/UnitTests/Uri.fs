module Test.Uri

open System
open Expecto
open Test

let tests =
  testList
    "Uri"
    [
      testCase
        "Render uri"
        (fun () ->
          serializeWithTypicalOptions (Uri("localhost:5000/test"))
          |> Expect.stringStart "\"localhost:5000/test\""
        )
      testCase
        "Render uri def"
        (fun () -> typedefof<Uri> |> definition |> Expect.stringStart "export type Uri = string")
      testCase
        "Render version"
        (fun () ->
          serializeWithTypicalOptions (System.Version("1.0.0.0"))
          |> Expect.stringStart "\"1.0.0.0\""

          serializeWithTypicalOptions (System.Version("1.0.0"))
          |> Expect.stringStart "\"1.0.0\""
          serializeWithTypicalOptions (System.Version("1.0"))
          |> Expect.stringStart "\"1.0\""
        )
      testCase
        "Render version def"
        (fun () ->
          typedefof<System.Version>
          |> definition
          |> Expect.stringStart "export type Version = string"
        )
    ]
