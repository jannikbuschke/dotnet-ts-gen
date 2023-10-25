module Test.Record

open System
open System.Text.Json.Serialization
open Expecto
open Test

type RecordWithPrimitiveOption =
  {
    Id: Guid
    numberOption: int option
  }

type MyOtherRecord = { Id: string }

type MyRecord =
  {
    Items: MyOtherRecord list
    Numbers: int list
  }

type EmptyRecord = { Skip: Skippable<unit> }

type SimpleRecord =
  {
    Id: Guid
    Name: string
    Number: int
    Obj: obj
  }

type ApiError =
  | Forbidden of string
  | BadRequest of string
  | InvalidState of string

type RecordWithResult = { Result: Result<MyRecord list, ApiError> }

let typedef2 = typedefof<RecordWithResult>

let tests =
  testList
    "Record"
    [
      testCase
        "Render record with primitive option"
        (fun () ->
          typedefof<RecordWithPrimitiveOption>
          |> definition
          |> Expect.equal
            """
export type RecordWithPrimitiveOption = {
  id: System.Guid
  numberOption: Microsoft_FSharp_Core.FSharpOption<System.Int32>
}
"""
        )
      testCase
        "Render Record with list"
        (fun () ->
          typedefof<MyRecord>
          |> definition
          |> Expect.equal
            """
export type MyRecord = {
  items: Microsoft_FSharp_Collections.FSharpList<MyOtherRecord>
  numbers: Microsoft_FSharp_Collections.FSharpList<System.Int32>
}
"""
        )
      testCase
        "Record with skippable"
        (fun () ->
          typeof<EmptyRecord>
          |> definition
          |> Expect.equal
            """
export type EmptyRecord = {
   skip: System_Text_Json_Serialization.Skippable<Microsoft_FSharp_Core.Unit>
}
"""
        )
      testCase
        "Render simple record"
        (fun () ->
          typedefof<SimpleRecord>
          |> definition
          |> Expect.equal
            """
export type SimpleRecord = {
  id: System.Guid
  name: System.String
  number: System.Int32
  obj: System.Object
}
"""
        )
      testCase
        "Render FSharpResult #2 - definition"
        (fun () ->
          typedef2
          |> definition
          |> Expect.similar
            """
export type RecordWithResult = {
  result: Microsoft_FSharp_Core.FSharpResult<Microsoft_FSharp_Collections.FSharpList<MyRecord>,ApiError>
}
"""
        )
    ]
