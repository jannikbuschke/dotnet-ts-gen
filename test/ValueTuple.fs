﻿module Test.ValueTuple

open Expecto
open Xunit

[<Fact>]
let ``Render tuple 2`` () =
  let resultType = typedefof<(int * string)>
  
  let rendered = renderTypeDef resultType

  Expect.similar
    rendered
    """
export type Tuple<T1,T2> = [T1,T2]
"""
  ()

[<Fact>]
let ``Render tuple 3`` () =
  let resultType = typedefof<(int * string * System.DateTime)>
  
  let rendered = renderTypeDef resultType

  Expect.similar
    rendered
    """
export type Tuple3<T1,T2,T3> = [T1,T2,T3]
"""
  ()
  
type X={Y:(int * string*System.DateTime)
           }
[<Fact>]
let ``Render tuple 3 property`` () =
  let resultType = typedefof<X>
  
  let rendered = renderTypeDef resultType

  Expect.similar
    rendered
    """
export type X = {
 y: System.Tuple3<System.Int32,System.String,System.DateTime>
}
"""
  ()
