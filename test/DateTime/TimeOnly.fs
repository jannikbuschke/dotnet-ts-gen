module Test.TimeOnly

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
      "\"09:05:00\""
     ]
    |> toTestInputs

[<Theory>]
[<MemberData(nameof testInputs)>]
let ``TimeOnly - serialized`` (encoding: JsonUnionEncoding) (expected: string) =
    let serialized = serializeWithEncoding encoding (TimeOnly.Parse("09:05"))
    Expect.similar serialized expected

let definition,value = renderTypeAndValue typedefof<TimeOnly>
[<Fact>]
let ``TimeOnly - definition`` () =

  Expect.similar
    definition
    """
export type TimeOnly = `${number}:${number}:${number}`
"""

// [<Fact>]
// let ``TimeOnly - value`` () =
//   Expect.similar
//     value
//     """
// export var defaultTimeOnly: TimeOnly = "00:00:00"
// """
