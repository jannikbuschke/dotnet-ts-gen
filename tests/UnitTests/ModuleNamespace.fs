module Test.ModuleTypes

open TinyTypeGen
open Expecto
open Test

module M1 =
  type M1Type = { Id: string }

module M2 =
  type M2Type = { Id: int }

let tests =
  testList
    "ModuleNamespace"
    [
      testCase
        "Render Module types in Module namespace"
        (fun () -> typedefof<M1.M1Type> |> getModuleName |> Expect.equal "Test_ModuleTypes_M1")
    ]
