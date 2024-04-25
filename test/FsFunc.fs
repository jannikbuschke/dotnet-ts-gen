module Test.FsFunc

open Expecto
open Xunit

type A = { Title:string;IsAllowed: unit -> bool }

[<Fact>]
let ```Func is ignored`` () =
  let rendered = renderTypeDef typeof<A>

  Expect.similar rendered "export type A = {
  title: System.String
}"

