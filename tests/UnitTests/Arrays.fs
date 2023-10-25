module Test.Arrays

open Expecto
open Test

type A = { Title: string }

type MyRecord =
  {
    Field1: string[]
    Field2: A[]
  }

let rendered = definition typedefof<MyRecord>

type RecordWithListOfStringArrayProperty = { Prop: ResizeArray<string array> }

let tests =
  testList
    "Arrays"
    [
      testCase
        "Render MyRecord with arrays"
        (fun () ->
          typedefof<MyRecord>
          |> definition
          |> Expect.equal
            """
export type MyRecord = {
 field1: System_Collections_Generic.IEnumerable<System.String>
 field2: System_Collections_Generic.IEnumerable<A>
}
"""
        )
      ptestCase
        "StringArray definition"
        (fun () ->
          let stringArray = typedefof<string array>

          stringArray
          |> definition
          |> Expect.equal
            """
export type StringArray = Array<String>  // fullname System.String[]
"""
        )
      testCase
        "Record with List<StringArray> prop definition"
        (fun () ->
          typedefof<RecordWithListOfStringArrayProperty>
          |> definition
          |> Expect.equal
            """
export type RecordWithListOfStringArrayProperty = {
 prop: System_Collections_Generic.List<Array<System.String>>
}
"""
        )
    ]
