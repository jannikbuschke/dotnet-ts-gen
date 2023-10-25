module Test.RecursiveType

open Expecto

type A = { B: B }
and B = { A: A }

type C = { Ds: D list }
and D = { G: G option }
and G = { Cs: C list }

let tests =
  testList
    "RecursiveType"
    [
      testCase
        "Render resursive type"
        (fun () ->
          typeof<A>
          |> definition
          |> Expect.similar
            "export type A = {
  b: B
}"

          typeof<B>
          |> definition
          |> Expect.similar
            "export type B = {
  a: A
}"
        )
      testCase
        "Render resursive type 2"
        (fun () ->
          typeof<C>
          |> definition
          |> Expect.similar
            "export type C = {
  ds: Microsoft_FSharp_Collections.FSharpList<D>
}"

          typeof<D>
          |> definition
          |> Expect.similar
            "export type D = {
  g: Microsoft_FSharp_Core.FSharpOption<G>
}"

          typeof<G>
          |> definition
          |> Expect.similar
            "export type G = {
  cs: Microsoft_FSharp_Collections.FSharpList<C>
}"
        )
    ]
