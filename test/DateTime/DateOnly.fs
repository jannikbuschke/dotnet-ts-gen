module Test.DateOnly

open System
open Expecto
open Xunit
open System
open Expecto
open Xunit
open System.Text.Json.Serialization
open Test
open Test
open Test.Unions.Utils

let testInputs =
    [
      // Unwrapped
      [ JsonUnionEncoding.Default
        JsonUnionEncoding.UnwrapSingleCaseUnions
        JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions
        JsonUnionEncoding.AdjacentTag
        JsonUnionEncoding.Inherit
        JsonUnionEncoding.NewtonsoftLike
        JsonUnionEncoding.UnwrapOption
        JsonUnionEncoding.AllowUnorderedTag
        JsonUnionEncoding.UnwrapFieldlessTags
        JsonUnionEncoding.UnionFieldNamesFromTypes
        
        JsonUnionEncoding.ExternalTag
        
        JsonUnionEncoding.ExternalTag ||| JsonUnionEncoding.UnwrapSingleFieldCases
        JsonUnionEncoding.InternalTag; JsonUnionEncoding.ThothLike
        JsonUnionEncoding.NamedFields
        JsonUnionEncoding.Untagged
        JsonUnionEncoding.UnwrapSingleFieldCases
         ],
      "\"2023-05-05\""
     ]
    |> toTestInputs

[<Theory>]
[<MemberData(nameof testInputs)>]
let ``DateOnly - serialized`` (encoding: JsonUnionEncoding) (expected: string) =
    let serialized = serializeWithEncoding encoding (DateOnly.Parse("2023-05-05"))
    Expect.similar serialized expected

let definition,value = renderTypeAndValue typedefof<DateOnly>
[<Fact>]
let ``DateOnly - definition`` () =

  Expect.similar
    definition
    """
export type DateOnly = `${number}-${number}-${number}`
"""

// [<Fact>]
// let ``DateOnly - value`` () =
//   Expect.similar
//     value
//     """
// export var defaultDateOnly: DateOnly = "0000-00-00"
// """
