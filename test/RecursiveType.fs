module Test.RecursiveType

open Expecto
open Xunit

type A = { B:B }
and B = { A:A }

[<Fact>]
let ```Render resursive type`` () =
  let rendered = renderTypeDef typeof<A>

  Expect.similar rendered "export type A = {
  b: B
}"
  let rendered2 = renderTypeDef typeof<B>

  Expect.similar rendered2 "export type B = {
  a: A
}"


type C = { Ds: D list }
and D = { G:G option }
and G = { Cs: C list }

[<Fact>]
let ```Render resursive type 2`` () =
  let rendered = renderTypeDef typeof<C>

  Expect.similar rendered "export type C = {
  ds: Microsoft_FSharp_Collections.FSharpList<D>
}"
  let rendered2 = renderTypeDef typeof<D>

  Expect.similar rendered2 "export type D = {
  g: Microsoft_FSharp_Core.FSharpOption<G>
}"

  let rendered2 = renderTypeDef typeof<G>

  Expect.similar rendered2 "export type G = {
  cs: Microsoft_FSharp_Collections.FSharpList<C>
}"
