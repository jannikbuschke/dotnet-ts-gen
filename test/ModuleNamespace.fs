module Test.ModuleTypes

open Expecto
open Xunit

module M1 =
  type M1Type = { Id: string }
module M2 =
  type M2Type = { Id: int }

[<Fact>]
let ``Render Module types in Module namespace`` () =
  let moduleName = TsGen.Signature.getModuleName typedefof<M1.M1Type>
  Expect.equal moduleName "Test_ModuleTypes_M1" "should equal"

