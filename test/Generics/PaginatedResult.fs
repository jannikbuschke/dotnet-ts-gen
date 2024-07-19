module TinyTypeGen.Test.Generics.PaginatedResult

open System.Collections.Generic
open Test.Unions
open Xunit
open Test

type PaginatedResult<'response> = {
  Items: IEnumerable<'response>
  Total: int
}
open System
open Expecto
open Xunit


[<Fact>]
let ``x`` () =
  let rendered = renderTypeDef typedefof<PaginatedResult<string>>
  let mName = TsGen.Signature.getModuleName(typedefof<PaginatedResult<string>>)

  Expect.similar
    rendered
    """
export type SimpleRecord = {
  id: System.Guid
  name: System.String
  number: System.Int32
  obj: System.Object
}
"""
