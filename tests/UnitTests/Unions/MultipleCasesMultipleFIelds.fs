module Test.Unions.MultipleCasesMultipleFields

open System.Text.Json.Serialization
open Expecto
open Test
open type JsonUnionEncoding

type Record = { Val: int }

[<RequireQualifiedAccess>]
type DuAnon =
  | Case1
  | AnonRecord of {| X: int |}

[<RequireQualifiedAccess>]
type Du =
  | Case1
  | Case2 of int
  | Case3 of Value1: int * string
  | Case4 of Record

let value1 = Du.Case1
let value2 = Du.Case2 0
let value3 = Du.Case3(0, "")
let value4 = Du.Case4 { Val = 0 }

let all =
  [
    JsonUnionEncoding.AdjacentTag
    JsonUnionEncoding.UnwrapOption
    JsonUnionEncoding.AllowUnorderedTag
    JsonUnionEncoding.UnwrapSingleCaseUnions
    JsonUnionEncoding.Inherit
    JsonUnionEncoding.NamedFields
    JsonUnionEncoding.UnwrapSingleFieldCases
    JsonUnionEncoding.UnionFieldNamesFromTypes

    JsonUnionEncoding.ExternalTag
    JsonUnionEncoding.InternalTag
    JsonUnionEncoding.Untagged
    JsonUnionEncoding.Inherit
    JsonUnionEncoding.UnwrapFieldlessTags
    JsonUnionEncoding.UnwrapOption
    JsonUnionEncoding.UnwrapRecordCases
    JsonUnionEncoding.AllowUnorderedTag
  ]

let fieldsArrayEncodings = [ JsonUnionEncoding.AdjacentTag ]

let value = [ value1; value2; value3; value4 ]

let valueAnon = [ DuAnon.AnonRecord {| X = 0 |}; DuAnon.Case1 ]

let tests =
  testList
    "Unions.MultipleCasesMultipleFields"
    [
      yield!
        fieldsArrayEncodings
        |> List.map (fun encoding ->
          testCase
            (sprintf "fields array - serialize (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.equal
                """ [{"Case":"Case1"},{"Case":"Case2","Fields":[0]},{"Case":"Case3","Fields":[0,""]},{"Case":"Case4","Fields":[{"val":0}]}] """
            )
        )

      yield!
        fieldsArrayEncodings
        |> List.map (fun encoding ->
          testCase
            (sprintf "fields array - typedef (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = { Case: "Case1", }
export type Du_Case_Case2 = { Case: "Case2", Fields: [ System.Int32 ] }
export type Du_Case_Case3 = { Case: "Case3", Fields: [ System.Int32, System.String ] }
export type Du_Case_Case4 = { Case: "Case4", Fields: [ Record ] }
"""
            )
        )

      testCase
        "AdjacentTag / NamedField - serialize"
        (fun () ->
          value
          |> serializeWithEncoding (NamedFields ||| AdjacentTag)
          |> Expect.equal
            """[{"Case":"Case1"},{"Case":"Case2","Fields":{"item":0}},{"Case":"Case3","Fields":{"value1":0,"item2":""}},{"Case":"Case4","Fields":{"item":{"val":0}}}]"""
        )

      testCase
        "AdjacentTag / NamedField - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (NamedFields ||| AdjacentTag)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case: "Case1", }
export type Du_Case_Case2 = { Case: "Case2", Fields: { item: System.Int32 } }
export type Du_Case_Case3 = { Case: "Case3", Fields: { value1: System.Int32, item2: System.String } }
export type Du_Case_Case4 = { Case: "Case4", Fields: { item: Record } }
"""
        )

      testCase
        "ExternalTag / NamedField - serialize"
        (fun () ->
          value
          |> serializeWithEncoding (NamedFields ||| ExternalTag)
          |> Expect.equal
            """[{"Case1":{}},{"Case2":{"item":0}},{"Case3":{"value1":0,"item2":""}},{"Case4":{"item":{"val":0}}}]"""
        )

      testCase
        "ExternalTag / NamedField - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (NamedFields ||| ExternalTag)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case1: {} }
export type Du_Case_Case2 = { Case2: { item: System.Int32 } }
export type Du_Case_Case3 = { Case3: { value1: System.Int32, item2: System.String } }
export type Du_Case_Case4 = { Case4: { item: Record } }
"""
        )

      testCase
        "ExternalTag - serialize"
        (fun () ->
          value
          |> serializeWithEncoding ExternalTag
          |> Expect.equal """ [{"Case1":[]},{"Case2":[0]},{"Case3":[0,""]},{"Case4":[{"val":0}]}] """
        )

      testCase
        "ExternalTag - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef ExternalTag
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case1: [ ] }
export type Du_Case_Case2 = { Case2: [ System.Int32 ] }
export type Du_Case_Case3 = { Case3: [ System.Int32, System.String ] }
export type Du_Case_Case4 = { Case4: [ Record ] }
"""
        )

      testCase
        "internal tag - serialize"
        (fun () ->
          value
          |> serializeWithEncoding JsonUnionEncoding.InternalTag
          |> Expect.equal """ [["Case1"],["Case2",0],["Case3",0,""],["Case4",{"val":0}]] """
        )

      testCase
        "internal tag - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef JsonUnionEncoding.InternalTag
          |> Expect.stringStart
            """
export type Du_Case_Case1 = [ "Case1", ]
export type Du_Case_Case2 = [ "Case2", System.Int32 ]
export type Du_Case_Case3 = [ "Case3", System.Int32, System.String ]
export type Du_Case_Case4 = [ "Case4", Record ]
"""
        )

      testCase
        "AdjacentTag / UnwrapSingleFieldCases - serialize"
        (fun () ->
          value
          |> serializeWithEncoding (AdjacentTag ||| UnwrapSingleFieldCases)
          |> Expect.equal
            """[{"Case":"Case1"},{"Case":"Case2","Fields":0},{"Case":"Case3","Fields":[0,""]},{"Case":"Case4","Fields":{"val":0}}]"""
        )

      testCase
        "AdjacentTag / UnwrapSingleFieldCases - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (AdjacentTag ||| UnwrapSingleFieldCases)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case: "Case1", }
export type Du_Case_Case2 = { Case: "Case2", Fields: System.Int32 }
export type Du_Case_Case3 = { Case: "Case3", Fields: [ System.Int32, System.String ] }
export type Du_Case_Case4 = { Case: "Case4", Fields: Record }
"""
        )

      testCase
        "ExternalTag / unwrap single field cases - serialize"
        (fun () ->
          value
          |> serializeWithEncoding (ExternalTag ||| UnwrapSingleFieldCases)
          |> Expect.equal """[{"Case1":[]},{"Case2":0},{"Case3":[0,""]},{"Case4":{"val":0}}]"""
        )

      testCase
        "ExternalTag / UnwrapSingleFieldCases - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (ExternalTag ||| UnwrapSingleFieldCases)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case1: [ ] }
export type Du_Case_Case2 = { Case2: System.Int32 }
export type Du_Case_Case3 = { Case3: [ System.Int32, System.String ] }
export type Du_Case_Case4 = { Case4: Record }
"""
        )

      testCase
        "InternalTag / unwrap single field cases - serialize"
        (fun () ->
          value
          |> serializeWithEncoding (InternalTag ||| UnwrapSingleFieldCases)
          |> Expect.equal """[["Case1"],["Case2",0],["Case3",0,""],["Case4",{"val":0}]]"""
        )

      testCase
        "InternalTag / UnwrapSingleFieldCases - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (InternalTag ||| UnwrapSingleFieldCases)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = [ "Case1", ]
export type Du_Case_Case2 = [ "Case2", System.Int32 ]
export type Du_Case_Case3 = [ "Case3", System.Int32, System.String ]
export type Du_Case_Case4 = [ "Case4", Record ]
"""
        )

      testCase
        "unwrap fieldless tag - serialize"
        (fun () ->
          value
          |> serializeWithEncoding JsonUnionEncoding.UnwrapFieldlessTags
          |> Expect.equal
            """["Case1",{"Case":"Case2","Fields":[0]},{"Case":"Case3","Fields":[0,""]},{"Case":"Case4","Fields":[{"val":0}]}]"""
        )

      testCase
        "Adjacent UnwrapFieldlessTag - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (AdjacentTag ||| UnwrapFieldlessTags)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = "Case1"
export type Du_Case_Case2 = { Case: "Case2", Fields: [ System.Int32 ] }
export type Du_Case_Case3 = { Case: "Case3", Fields: [ System.Int32, System.String ] }
export type Du_Case_Case4 = { Case: "Case4", Fields: [ Record ] }
"""
        )

      testCase
        "unwrap single case union - serialize"
        (fun () ->
          value
          |> serializeWithEncoding (AdjacentTag ||| UnwrapSingleCaseUnions)
          |> Expect.equal
            """[{"Case":"Case1"},{"Case":"Case2","Fields":[0]},{"Case":"Case3","Fields":[0,""]},{"Case":"Case4","Fields":[{"val":0}]}]"""
        )

      testCase
        "Adjacent UnwrapSingleCaseUnion - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (AdjacentTag ||| UnwrapSingleCaseUnions)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case: "Case1", }
export type Du_Case_Case2 = { Case: "Case2", Fields: [ System.Int32 ] }
export type Du_Case_Case3 = { Case: "Case3", Fields: [ System.Int32, System.String ] }
export type Du_Case_Case4 = { Case: "Case4", Fields: [ Record ] }
"""
        )

      testCase
        "fsharplu - serialize"
        (fun () ->
          value
          |> serializeWithEncoding FSharpLuLike
          |> Expect.equal """["Case1",{"Case2":0},{"Case3":[0,""]},{"Case4":{"val":0}}]"""
        )

      testCase
        "fsharplu - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef FSharpLuLike
          |> Expect.stringStart
            """
export type Du_Case_Case1 = "Case1"
export type Du_Case_Case2 = { Case2: System.Int32 }
export type Du_Case_Case3 = { Case3: [ System.Int32, System.String ] }
export type Du_Case_Case4 = { Case4: Record }
"""
        )

      testCase
        "tothlike - serialize"
        (fun () ->
          value
          |> serializeWithEncoding JsonUnionEncoding.ThothLike
          |> Expect.equal """["Case1",["Case2",0],["Case3",0,""],["Case4",{"val":0}]]"""
        )

      testCase
        "tothlike - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef JsonUnionEncoding.ThothLike
          |> Expect.stringStart
            """
export type Du_Case_Case1 = "Case1"
export type Du_Case_Case2 = [ "Case2", System.Int32 ]
export type Du_Case_Case3 = [ "Case3", System.Int32, System.String ]
export type Du_Case_Case4 = [ "Case4", Record ]
"""
        )

      testCase
        "AdjacentTag / UnwrapOption - serialize"
        (fun () ->
          value
          |> serializeWithEncoding (AdjacentTag ||| UnwrapOption)
          |> Expect.equal
            """[{"Case":"Case1"},{"Case":"Case2","Fields":[0]},{"Case":"Case3","Fields":[0,""]},{"Case":"Case4","Fields":[{"val":0}]}]"""
        )

      ptestCase
        "AdjacentTag / UnwrapOption - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (AdjacentTag ||| UnwrapOption)
          |> Expect.equal
            """
export type Du_Case_Case1 = "Case1"
export type Du_Case_Case2 = { Case: "Case2", Fields: [ System.Int32 ] }
export type Du_Case_Case3 = { Case: "Case3", Fields: [ System.Int32, System.String ] }
export type Du_Case_Case4 = { Case: "Case4", Fields: [ Record ] }
"""
        )

      testCase
        "unwrap option - serialize option"
        (fun () ->
          [ Some 5, None, Some {| Foo = 5 |} ]
          |> serializeWithEncoding JsonUnionEncoding.UnwrapOption
          |> Expect.equal """[[5,null,{"foo":5}]]"""
        )

      testCase
        "AdjacentTag / UnwrapRecordCases - serialize"
        (fun () ->
          value
          |> serializeWithEncoding (AdjacentTag ||| UnwrapRecordCases)
          |> Expect.equal
            """[{"Case":"Case1"},{"Case":"Case2","Fields":{"item":0}},{"Case":"Case3","Fields":{"value1":0,"item2":""}},{"Case":"Case4","Fields":{"val":0}}]"""
        )

      testCase
        "AdjacentTag / UnwrapRecordCases - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (AdjacentTag ||| UnwrapRecordCases)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case: "Case1", }
export type Du_Case_Case2 = { Case: "Case2", Fields: { item: System.Int32 } }
export type Du_Case_Case3 = { Case: "Case3", Fields: { value1: System.Int32, item2: System.String } }
export type Du_Case_Case4 = { Case: "Case4", Fields: Record }
"""
        )

      testCase
        "ExternalTag / UnwrapRecordCase - serialize"
        (fun () ->
          value
          |> serializeWithEncoding (ExternalTag ||| UnwrapRecordCases)
          |> Expect.equal
            """[{"Case1":{}},{"Case2":{"item":0}},{"Case3":{"value1":0,"item2":""}},{"Case4":{"val":0}}]"""
        )

      testCase
        "AdjacentTag / UnwrapRecordCases, anonymous record case - serialize"
        (fun () ->
          [ valueAnon ]
          |> serializeWithEncoding (AdjacentTag ||| UnwrapRecordCases)
          |> Expect.equal """[[{"Case":"AnonRecord","Fields":{"x":0}},{"Case":"Case1"}]]"""
        )

      testCase
        "AdjacentTag / UnwrapRecordCases, anonymous record case - typedef"
        (fun () ->
          typedefof<DuAnon>
          |> renderCustomTypeDef (AdjacentTag ||| UnwrapRecordCases)
          |> Expect.stringStart
            """
export type DuAnon_Case_Case1 = { Case: "Case1", }
export type DuAnon_Case_AnonRecord = { Case: "AnonRecord", Fields: ___.f__AnonymousType3907696548<System.Int32> }
"""
        )

      testCase
        "ExternalTag / UnwrapRecordCase - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (ExternalTag ||| UnwrapRecordCases)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case1: {} }
export type Du_Case_Case2 = { Case2: { item: System.Int32 } }
export type Du_Case_Case3 = { Case3: { value1: System.Int32, item2: System.String } }
export type Du_Case_Case4 = { Case4: Record }
"""
        )

      testCase
        "InternalTag UnwrapRecordCase - serialize"
        (fun () ->
          value
          |> serializeWithEncoding (InternalTag ||| UnwrapRecordCases)
          |> Expect.equal
            """[{"Case":"Case1"},{"Case":"Case2","item":0},{"Case":"Case3","value1":0,"item2":""},{"Case":"Case4","val":0}]"""
        )

      testCase
        "InternalTag UnwrapRecordCase - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (InternalTag ||| UnwrapRecordCases)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case: "Case1", }
export type Du_Case_Case2 = { Case: "Case2", item: System.Int32 }
export type Du_Case_Case3 = { Case: "Case3", value1: System.Int32, item2: System.String }
export type Du_Case_Case4 = { Case: "Case4" } & Record
"""
        )

      testCase
        "InternalTag UnwrapRecordCase, anon - serialize"
        (fun () ->
          valueAnon
          |> serializeWithEncoding (InternalTag ||| UnwrapRecordCases)
          |> Expect.equal """[{"Case":"AnonRecord","x":0},{"Case":"Case1"}]"""
        )

      testCase
        "InternalTag UnwrapRecordCase, anon - typedef"
        (fun () ->
          typedefof<DuAnon>
          |> renderCustomTypeDef (InternalTag ||| UnwrapRecordCases)
          |> Expect.stringStart
            """
export type DuAnon_Case_Case1 = { Case: "Case1", }
export type DuAnon_Case_AnonRecord = { Case: "AnonRecord" } & ___.f__AnonymousType3907696548<System.Int32> 
"""
        )

      testCase
        "InternalTag NamedFields - serialize"
        (fun () ->
          value
          |> serializeWithEncoding (InternalTag ||| NamedFields)
          |> Expect.equal
            """[{"Case":"Case1"},{"Case":"Case2","item":0},{"Case":"Case3","value1":0,"item2":""},{"Case":"Case4","item":{"val":0}}]"""
        )

      testCase
        "InternalTag NamedFields - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (InternalTag ||| NamedFields)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case: "Case1", }
export type Du_Case_Case2 = { Case: "Case2", item: System.Int32 }
export type Du_Case_Case3 = { Case: "Case3", value1: System.Int32, item2: System.String }
export type Du_Case_Case4 = { Case: "Case4", item: Record }
"""
        )

      testCase
        "InternalTag NamedFields, anon record case - serialize"
        (fun () ->
          valueAnon
          |> serializeWithEncoding (InternalTag ||| NamedFields)
          |> Expect.equal """[{"Case":"AnonRecord","item":{"x":0}},{"Case":"Case1"}]"""
        )

      testCase
        "InternalTag NamedFields, anon record case - typedef"
        (fun () ->
          typedefof<DuAnon>
          |> renderCustomTypeDef (InternalTag ||| NamedFields)
          |> Expect.stringStart
            """
export type DuAnon_Case_Case1 = { Case: "Case1", }
export type DuAnon_Case_AnonRecord = { Case: "AnonRecord", item: ___.f__AnonymousType3907696548<System.Int32> }
"""
        )
    ]
