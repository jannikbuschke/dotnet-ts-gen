module Test.Unions.GenericDu

open Expecto
open Xunit
open Test

type AbstractCondition<'T> =
  | Or of AbstractCondition<'T> * AbstractCondition<'T>
  | And of AbstractCondition<'T> * AbstractCondition<'T>
  | Expression of 'T

[<Fact>]
let ``Result property`` () =
  let rendered = renderTypeDef typedefof<AbstractCondition<_>>

  Expect.similar
    rendered
    """
export type AbstractCondition_Case_Or<T> = { Case: "Or", Fields: { item1: AbstractCondition<T>, item2: AbstractCondition<T> } }
export type AbstractCondition_Case_And<T> = { Case: "And", Fields: { item1: AbstractCondition<T>, item2: AbstractCondition<T> } }
export type AbstractCondition_Case_Expression<T> = { Case: "Expression", Fields: T }
export type AbstractCondition<T> = AbstractCondition_Case_Or<T> | AbstractCondition_Case_And<T> | AbstractCondition_Case_Expression<T>
export type AbstractCondition_Case = "Or" | "And" | "Expression"
"""
