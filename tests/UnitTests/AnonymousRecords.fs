module Test.AnonymousRecord

open Expecto
open Test

type AnonymousRecord =
  {|
    Id: string
    Name: string
  |}

type RecordWithAnonymousField =
  {
    Field:
      {|
        Id: string
        Name: string
      |}
  }

let tests =
  testList
    "AnonymousRecords"
    [
      testCase
        "Render AnonymousRecord"
        (fun () ->
          typedefof<AnonymousRecord>
          |> definition
          |> Expect.equal
            """
export type f__AnonymousType650789795<Id,Name> = {
 id: Id
 name: Name
}
"""
        )
      testCase
        "Render module of Record with AnonymousRecord field"
        (fun () ->
          typeof<RecordWithAnonymousField>
          |> renderModule
          |> Expect.equal
            """
//////////////////////////////////////
// This file is auto generated //
//////////////////////////////////////
import * as ___ from "./___"
import * as System from "./System"
export type RecordWithAnonymousField = {
 field: ___.f__AnonymousType650789795<System.String,System.String>
}
"""
        )
    ]
