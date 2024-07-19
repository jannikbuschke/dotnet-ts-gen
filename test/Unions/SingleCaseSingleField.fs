module Test.Unions.SingleCaseSingleField

open System
open Expecto
open Xunit
open System.Text.Json.Serialization
open Test

type MyRecordId = MyRecordId of Guid
type MyRecord = { Id: MyRecordId }

let testInputs =
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
      "{\"Case\":\"MyRecordId\",\"Fields\":{\"item\":\"00000000-0000-0000-0000-000000000000\"}}"
      // Untagged
      [ JsonUnionEncoding.Untagged ], "{\"item\":\"00000000-0000-0000-0000-000000000000\"}"
      // Single fields are unwrapped
      [ JsonUnionEncoding.UnwrapSingleFieldCases ],
      "{\"Case\":\"MyRecordId\",\"Fields\":\"00000000-0000-0000-0000-000000000000\"}" ]
    |> toTestInputs

[<Theory>]
[<MemberData(nameof testInputs)>]
let ``Union - single case, single field - serialized`` (encoding: JsonUnionEncoding) (expected: string) =
    let serialized = serializeWithEncoding encoding (MyRecordId.MyRecordId Guid.Empty)
    Expect.similar serialized expected


[<Fact>]
let ``Union - single case, single field - unwrapped - definition`` () =
    let typedef, value = renderTypeAndValue typedefof<MyRecordId>
    Expect.similar
        typedef
        """
export type MyRecordId_Case_MyRecordId = System.Guid
export type MyRecordId = MyRecordId_Case_MyRecordId
export type MyRecordId_Case = "MyRecordId"
"""

// [<Fact>]
// let ``Union - single case, single field - unwrapped - value`` () =
//     Expect.similar
//         value
//         """
// export var MyRecordId_AllCases = [ "MyRecordId" ] as const
// export var defaultMyRecordId_Case_MyRecordId = '00000000-0000-0000-0000-000000000000'
// export var defaultMyRecordId = defaultMyRecordId_Case_MyRecordId as MyRecordId
// """


[<Fact>]
let ``Record - single case and single value property - definition`` () =
    let typedef2, renderedValue2 = renderTypeAndValue typedefof<MyRecord>
    Expect.similar
        typedef2
        """
export type MyRecord = {
  id: MyRecordId
}
"""

// [<Fact>]
// let ``Record - single case and single value property - value`` () =
//     Expect.similar
//         renderedValue2
//         """
// export var defaultMyRecord: MyRecord = {
//   id: defaultMyRecordId
// }
// """
