module Test.Unions.FSharpResult

open Expecto
open Xunit
open Test


[<Fact>]
let ``Render FSharpResult`` () =
  let typedef = typedefof<Result<string, string>>
  let rendered = renderTypeDef typedef

  Expect.similar
    rendered
    """
export type FSharpResult_Case_Ok<T> = { Case: "Ok", Fields: T }

export type FSharpResult_Case_Error<TError> = { Case: "Error", Fields: TError }

export type FSharpResult<T,TError> = FSharpResult_Case_Ok<T> | FSharpResult_Case_Error<TError>

export type FSharpResult_Case = "Ok" | "Error"
"""

// [<Fact>]
// let ``Render FSharpResult value`` () =
//   let rendered = renderValue typedef
//
//   Expect.similar
//     rendered
//     """
// export var FSharpResult_AllCases = [ "Ok", "Error" ] as const
// export var defaultFSharpResult_Case_Ok = <T,TError>(defaultT:T,defaultTError:TError) => ({ Case: "Ok", Fields: defaultT })
// export var defaultFSharpResult_Case_Error = <T,TError>(defaultT:T,defaultTError:TError) => ({ Case: "Error", Fields: defaultTError })
// export var defaultFSharpResult = <T,TError>(defaultT:T,defaultTError:TError) => defaultFSharpResult_Case_Ok(defaultT,defaultTError) as FSharpResult<T,TError>
// """

type MyRecord = { Name: string }

type ApiError =
  | Forbidden of string
  | BadRequest of string
  | InvalidState of string

type RecordWithResult =
  { Result: Result<MyRecord list, ApiError> }

let typedef2 = typedefof<RecordWithResult>

[<Fact>]
let ``Render FSharpResult #2 - definition`` () =
  let rendered = renderTypeDef typedef2

  Expect.similar
    rendered
    """
export type RecordWithResult = {
  result: Microsoft_FSharp_Core.FSharpResult<Microsoft_FSharp_Collections.FSharpList<MyRecord>,ApiError>
}
"""
//
// [<Fact>]
// let ``Render FSharpResult #2 - value`` () =
//   let rendered = renderValue typedef2
//
//   Expect.similar
//     rendered
//     """
// export var defaultRecordWithResult: RecordWithResult = {
//   result: Microsoft_FSharp_Core.defaultFSharpResult(Microsoft_FSharp_Collections.defaultFSharpList(defaultMyRecord),defaultApiError)
// }
// """
