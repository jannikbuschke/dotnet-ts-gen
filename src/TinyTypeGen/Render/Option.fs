module TinyTypeGen.Option

open System.Text.Json.Serialization
open type JsonUnionEncoding
open TinyTypeGen.Render.Unions

let render (encoding: JsonUnionEncoding) =
  let definition =
    if encoding.HasFlag UnwrapOption then
      "T | null"
    else
      match encoding with
      | Adjacent & UnwrapSingleFieldCase -> """{ Case: "Some", Fields: T } | null"""
      | Adjacent & NamedFields -> """{ Case: "Some", Fields: { value: T } } | null"""
      | Adjacent -> """{ Case: "Some", Fields: [T] } | null"""
      | External & UnwrapSingleFieldCase -> "{ Some: T } | null"
      | External & NamedFields -> "{ Some: { value: T } } | null"
      | External -> "{Some:T[] } | null"
      | Internal & NamedFields -> """{ Case: "Some", value: T } | null"""
      | Internal & UnwrapSingleFieldCase -> """["Some", T] | null"""
      | Internal -> """["Some", T] | null"""
      | Internal & UnwrapSingleFieldCase -> """{ Case: "Some", value: T } | null"""
      | _ -> """{ Case: "Some"; Fields: [T] } | null"""

  let variant =
    if encoding.HasFlag UnwrapRecordCases then
      if encoding.HasFlag UnwrapOption then
        """export type FSharpOption_T<T> = T | null"""
      else if encoding.HasFlag InternalTag then
        """export type FSharpOption_T<T> = { Case: "Some" } & T | null"""
      else if encoding.HasFlag AdjacentTag then
        if encoding.HasFlag UnwrapOption then
          """export type FSharpOption_T<T> = T | null"""
        else
          """export type FSharpOption_T<T> = { Case: "Some", Fields: T } | null"""
      else if encoding.HasFlag ExternalTag then
        """export type FSharpOption_T<T> = { Some: T } | null"""
      else
        ""
    else
      ""

  $"""export type FSharpOption<T> = {definition}
{variant}
"""
