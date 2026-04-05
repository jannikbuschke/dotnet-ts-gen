module Test.Unions.GenericDu

open Expecto
open TinyTypeGen
open Test

type AbstractCondition<'T> =
  | Or of AbstractCondition<'T> * AbstractCondition<'T>
  | And of AbstractCondition<'T> * AbstractCondition<'T>
  | Expression of 'T

type Du<'x, 'y, 'z> =
  | X of 'x
  | Y of 'x * 'y
  | Z of 'x * string * 'z
  | U of
    {|
      X: 'x
      Name: string
    |}

let value: obj list = [ Du.Z(Some 5, "test", None) ]

let check value typedef encoding expectedJson expectedDefinition =
  value |> serializeWithEncoding encoding |> Expect.equal expectedJson
  typedef |> renderCustomTypeDef encoding |> Expect.stringStart expectedDefinition

let test = check value typedefof<Du<_, _, _>>

open System.Text.Json.Serialization
open type JsonUnionEncoding

let tests =
  testList
    "Unions.GenericDu"
    [
      testCase
        "Recursive generic du property"
        (fun () ->
          typedefof<AbstractCondition<_>>
          |> renderCustomTypeDef Config.defaultJsonUnionEncoding
          |> Expect.stringStart
            """
export type AbstractCondition_Case_Or<T> = { Case: "Or", Fields: { item1: AbstractCondition<T>, item2: AbstractCondition<T> } }
export type AbstractCondition_Case_And<T> = { Case: "And", Fields: { item1: AbstractCondition<T>, item2: AbstractCondition<T> } }
export type AbstractCondition_Case_Expression<T> = { Case: "Expression", Fields: T }
export type AbstractCondition<T> = AbstractCondition_Case_Or<T> | AbstractCondition_Case_And<T> | AbstractCondition_Case_Expression<T>
"""
        )

      testCase
        "CustomEncoding - Generic Du multi case - serialize"
        (fun () ->
          [
            Du.X ""
            Du.Y("", "")
            Du.Z("", "", "")
            Du.U
              {|
                X = ""
                Name = ""
              |}
          ]
          |> serializeWithEncoding Config.defaultJsonUnionEncoding
          |> Expect.stringStart
            """
[{"Case":"X","Fields":""},{"Case":"Y","Fields":{"item1":"","item2":""}},{"Case":"Z","Fields":{"item1":"","item2":"","item3":""}},{"Case":"U","Fields":{"name":"","x":""}}]
"""
        )

      testCase
        "CustomEncoding - Generic Du multi case"
        (fun () ->
          typedefof<Du<_, _, _>>
          |> renderCustomTypeDef Config.defaultJsonUnionEncoding
          |> Expect.stringStart
            """
export type Du_Case_X<x,y,z> = { Case: "X", Fields: x }
export type Du_Case_Y<x,y,z> = { Case: "Y", Fields: { item1: x, item2: y } }
export type Du_Case_Z<x,y,z> = { Case: "Z", Fields: { item1: x, item2: System.String, item3: z } }
export type Du_Case_U<x,y,z> = { Case: "U", Fields: ___.f__AnonymousType2542291838<System.String,x> }
"""
        )

      testCase
        "_AdjacentTag_NamedFields_UnwrapFieldlessTags_UnwrapSingleCaseUnions_UnwrapSingleFieldCases"
        (fun () ->
          test
            (AdjacentTag
             ||| NamedFields
             ||| UnwrapFieldlessTags
             ||| UnwrapSingleCaseUnions
             ||| UnwrapSingleFieldCases)
            """[{"Case":"Z","Fields":{"item1":{"Case":"Some","Fields":5},"item2":"test","item3":null}}]"""
            """export type Du_Case_X<x,y,z> = { Case: "X", Fields: x }
export type Du_Case_Y<x,y,z> = { Case: "Y", Fields: { item1: x, item2: y } }
export type Du_Case_Z<x,y,z> = { Case: "Z", Fields: { item1: x, item2: System.String, item3: z } }
export type Du_Case_U<x,y,z> = { Case: "U", Fields: ___.f__AnonymousType2542291838<System.String,x> }
export type Du<x,y,z> = Du_Case_X<x,y,z> | Du_Case_Y<x,y,z> | Du_Case_Z<x,y,z> | Du_Case_U<x,y,z>
export type Du_Case = "X" | "Y" | "Z" | "U"
export const Du_AllCases = [
 "X",
 "Y",
 "Z",
 "U"] satisfies Du_Case[]
export function isDu_Case(value: any): value is Du_Case {
 return Du_AllCases.includes(value)
}"""
        )
    ]
