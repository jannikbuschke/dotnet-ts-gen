module Test.SingleCaseDu

open System
open Expecto
open TsGen
open Xunit
open System.Text.Json.Serialization

// let options = DefaultSerialize.JsonFSharpOptions
let options = JsonFSharpOptions(Gen.defaultJsonUnionEncoding)
let newtonsoftLike = JsonFSharpOptions.NewtonsoftLike()

let serialize<'t> = serializeWithOptions<'t> options
let deserialize<'t> = deserializeWithOptions<'t> options

type MyRecordId = MyRecordId of Guid
type MyRecord = { Id: MyRecordId }

let serialized0, serialized1 =
    serialize (MyRecordId.MyRecordId(Guid.Empty)), serialize { Id = MyRecordId.MyRecordId(Guid.Empty) }

let typedef, value = renderTypeAndValue typedefof<MyRecordId>

type SerializeTestParams = (JsonUnionEncoding * string) list

let withExpectedOutput inputs (expected: string) =
    inputs |> List.map (fun v -> (v, expected))

let internalTag =
    [
      // Unwrapped
      [ JsonUnionEncoding.Default
        JsonUnionEncoding.UnwrapSingleCaseUnions
        JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions ],
      "\"00000000-0000-0000-0000-000000000000\""
      // Fields as array
      [ JsonUnionEncoding.AdjacentTag
        JsonUnionEncoding.Inherit
        JsonUnionEncoding.NewtonsoftLike
        JsonUnionEncoding.UnwrapOption
        JsonUnionEncoding.AllowUnorderedTag
        JsonUnionEncoding.UnwrapFieldlessTags
        JsonUnionEncoding.UnionFieldNamesFromTypes ],
      "{\"Case\":\"MyRecordId\",\"Fields\":[\"00000000-0000-0000-0000-000000000000\"]}"
      // External Tag, Fields as Array
      [ JsonUnionEncoding.ExternalTag ], "{\"MyRecordId\":[\"00000000-0000-0000-0000-000000000000\"]}"
      [ JsonUnionEncoding.ExternalTag ||| JsonUnionEncoding.UnwrapSingleFieldCases ],
      "{\"MyRecordId\":\"00000000-0000-0000-0000-000000000000\"}"
      // Array of tuples
      [ JsonUnionEncoding.InternalTag; JsonUnionEncoding.ThothLike ],
      "[\"MyRecordId\",\"00000000-0000-0000-0000-000000000000\"]"
      // Fields as record
      [ JsonUnionEncoding.NamedFields ],
      "{\"Case\":\"MyRecordId\",\"Fields\":{\"Item\":\"00000000-0000-0000-0000-000000000000\"}}"
      // Untagged
      [ JsonUnionEncoding.Untagged ], "{\"Item\":\"00000000-0000-0000-0000-000000000000\"}"
      // Single fields are unwrapped
      [ JsonUnionEncoding.UnwrapSingleFieldCases ],
      "{\"Case\":\"MyRecordId\",\"Fields\":\"00000000-0000-0000-0000-000000000000\"}" ]

let testInputs =
    internalTag
    |> List.collect (fun (encodings, expected) ->
        encodings
        |> List.map (fun (encoding) -> [| (encoding :> obj); (expected :> obj) |]))

[<Theory>]
[<MemberData(nameof testInputs)>]
let ``Union - single case, single field - serialized`` (encoding: JsonUnionEncoding) (expected: string) =
    let serialized = serializeWithEncoding encoding (MyRecordId.MyRecordId Guid.Empty)
    Expect.similar serialized expected

[<Fact>]
let ``Union - single case, single field - unwrapped - definition`` () =
    Expect.similar
        value
        """
export type MyRecordId_Case_MyRecordId = System.Guid
export type MyRecordId = MyRecordId_Case_MyRecordId
export type MyRecordId_Case = "MyRecordId"
"""

[<Fact>]
let ``Union - single case, single field - unwrapped - value`` () =
    Expect.similar
        typedef
        """
export var MyRecordId_AllCases = [ "MyRecordId" ] as const
export var defaultMyRecordId_Case_MyRecordId = '00000000-0000-0000-0000-000000000000'
export var defaultMyRecordId = defaultMyRecordId_Case_MyRecordId as MyRecordId
"""

let typedef2, renderedValue2 = renderTypeAndValue typedefof<MyRecord>

[<Fact>]
let ``Record - single case and single value property - definition`` () =
    Expect.similar
        typedef2
        """
export type MyRecord = {
  id: MyRecordId
}
"""

[<Fact>]
let ``Record - single case and single value property - value`` () =
    Expect.similar
        renderedValue2
        """
export var defaultMyRecord: MyRecord = {
  id: defaultMyRecordId
}
"""
