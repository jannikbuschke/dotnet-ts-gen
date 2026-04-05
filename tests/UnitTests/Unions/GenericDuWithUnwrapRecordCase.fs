module Test.Unions.GenericDuWithUnwrapRecordCase

// If unwrap record case is enabled, and we have generic discriminated union
// we dont know from the generic type allone if the field will be a record or a primitive type
// we need to check the actual type to decide how to render it.
// Creating one generic type in typescript that handles all cases properly is not directly possible
// as typescript is structural typed and two types with same structure are considered the same type
// at typescript compile time it is impossible to differentiate records and unions for example
// So we will generate different generic types depending on how they are used

open Expecto
open Test
open System.Text.Json.Serialization
open type JsonUnionEncoding

[<RequireQualifiedAccess>]
type GenericDu<'T> = | CaseA of 'T

[<RequireQualifiedAccess>]
type GenericDu3<'a, 'b, 'c> =
  | CaseA of 'a
  | CaseB of 'b
  | CaseC of 'c
  | CaseAb of 'a * 'b

type Record = { Val: int }
let g3 =
  [
    GenericDu3<Record, Record, string>.CaseA { Val = 3 }
    GenericDu3.CaseB { Val = 3 }
    GenericDu3.CaseC ""
    GenericDu3.CaseAb({ Val = 3 }, { Val = 0 })
  ]

[<RequireQualifiedAccess>]
type GenericDuWithRecord<'t> =
  | Record of Record
  | Case2 of 't

let duWithRecord: obj list = [ GenericDu.CaseA { Val = 0 } ]
let duWithPrimitive: obj list = [ GenericDu.CaseA 5 ]
let duWithDu: obj list = [ GenericDu.CaseA(Ok "") ]
let duWithRecord2: obj list =
  [ GenericDuWithRecord.Record { Val = 0 }; GenericDuWithRecord.Case2 5 ]
let duWithRecord3: obj list =
  [
    GenericDuWithRecord.Record { Val = 0 }
    GenericDuWithRecord.Case2 { Val = 1 }
  ]

type ReferencingTypeWithRecord = { Val: GenericDu<Record> }
type ReferencingTypeWithPrimitive = { Val: GenericDu<int> }
type ReferencingTypeWithUnion = { Val: GenericDu<Result<string, string>> }

type ReferencingType =
  {
    V1: GenericDuWithRecord<Record>
    V2: GenericDuWithRecord<int>
  }

type ReferencingDu3 =
  {
    V_primitives: GenericDu3<string, string, string>
    V_record_a: GenericDu3<Record, string, string>
    V_record_ab: GenericDu3<Record, Record, string>
    V_record_abc: GenericDu3<Record, Record, Record>
  }
let referencingDu3Value =
  {
    V_primitives = GenericDu3.CaseA "test"
    V_record_a = GenericDu3.CaseB "test"
    V_record_ab = GenericDu3.CaseC "test"
    V_record_abc = GenericDu3.CaseB { Val = 0 }
  }

let referencingWithRecord: ReferencingTypeWithRecord list =
  [ { Val = GenericDu.CaseA { Val = 0 } } ]
let referencingWithPrimitive: ReferencingTypeWithPrimitive list =
  [ { Val = GenericDu.CaseA 5 } ]
let referencingWithUnion: ReferencingTypeWithUnion list =
  [ { Val = GenericDu.CaseA(Ok "") } ]
let referencingMultiGeneric: ReferencingTypeWithUnion list =
  [ { Val = GenericDu.CaseA(Ok "") } ]
let referencingTypeWithRecord2: ReferencingType list = []

let check value typedef encoding expectedJson expectedDefinition =
  value |> serializeWithEncoding encoding |> Expect.equal expectedJson
  typedef |> renderCustomTypeDef encoding |> Expect.stringStart expectedDefinition

let checkReferencingDu3 = check referencingDu3Value typedefof<ReferencingDu3>
let testDuWithRecord = check duWithRecord typedefof<GenericDu<_>>
let testDuWithPrimitive = check duWithPrimitive typedefof<GenericDu<_>>
let testDuWithDu = check duWithDu typedefof<GenericDu<_>>
let testRefencingWithRecord =
  check referencingWithRecord typedefof<ReferencingTypeWithRecord>
let testRefencingWithPrimitive =
  check referencingWithPrimitive typedefof<ReferencingTypeWithPrimitive>
let testRefencingWithUnion =
  check referencingWithUnion typedefof<ReferencingTypeWithUnion>

let testMultiGeneric = check g3 typedefof<GenericDu3<_, _, _>>

let tests =
  testList
    "Unions.GenericDuWithUnwrapRecordCase"
    [
      testCase
        "Referencing multi du"
        (fun () ->
          checkReferencingDu3
            (InternalTag ||| UnwrapRecordCases)
            """{"v_primitives":{"Case":"CaseA","item":"test"},"v_record_a":{"Case":"CaseB","item":"test"},"v_record_ab":{"Case":"CaseC","item":"test"},"v_record_abc":{"Case":"CaseB","val":0}}"""
            """export type ReferencingDu3 = {
 v_primitives: GenericDu3<System.String,System.String,System.String>
 v_record_a: GenericDu3_a<Record,System.String,System.String>
 v_record_ab: GenericDu3_ab<Record,Record,System.String>
 v_record_abc: GenericDu3_abc<Record,Record,Record>
}
"""
        )

      testCase
        "Multi generic test"
        (fun () ->
          testMultiGeneric
            (InternalTag ||| UnwrapRecordCases)
            """[{"Case":"CaseA","val":3},{"Case":"CaseB","val":3},{"Case":"CaseC","item":""},{"Case":"CaseAb","item1":{"val":3},"item2":{"val":0}}]"""
            """export type GenericDu3_Case_CaseA<a,b,c> = { Case: "CaseA", item: a }
export type GenericDu3_Case_CaseB<a,b,c> = { Case: "CaseB", item: b }
export type GenericDu3_Case_CaseC<a,b,c> = { Case: "CaseC", item: c }
export type GenericDu3_Case_CaseAb<a,b,c> = { Case: "CaseAb", item1: a, item2: b }
export type GenericDu3<a,b,c> = GenericDu3_Case_CaseA<a,b,c> | GenericDu3_Case_CaseB<a,b,c> | GenericDu3_Case_CaseC<a,b,c> | GenericDu3_Case_CaseAb<a,b,c>
export type GenericDu3_Case = "CaseA" | "CaseB" | "CaseC" | "CaseAb"
export const GenericDu3_AllCases = [
 "CaseA",
 "CaseB",
 "CaseC",
 "CaseAb"] satisfies GenericDu3_Case[]
export function isGenericDu3_Case(value: any): value is GenericDu3_Case {
 return GenericDu3_AllCases.includes(value)
}
// GENERIC RECORD DU: a
export type GenericDu3_Case_CaseA_a<a,b,c> = { Case: "CaseA" } & a
export type GenericDu3_Case_CaseB_a<a,b,c> = { Case: "CaseB", item: b }
export type GenericDu3_Case_CaseC_a<a,b,c> = { Case: "CaseC", item: c }
export type GenericDu3_Case_CaseAb_a<a,b,c> = { Case: "CaseAb", item1: a, item2: b }
export type GenericDu3_a<a,b,c> = GenericDu3_Case_CaseA_a<a,b,c> | GenericDu3_Case_CaseB_a<a,b,c> | GenericDu3_Case_CaseC_a<a,b,c> | GenericDu3_Case_CaseAb_a<a,b,c>
export type GenericDu3_a_Case = "CaseA" | "CaseB" | "CaseC" | "CaseAb"
export const GenericDu3_a_AllCases = [
 "CaseA",
 "CaseB",
 "CaseC",
 "CaseAb"] satisfies GenericDu3_a_Case[]
export function isGenericDu3_a_Case(value: any): value is GenericDu3_a_Case {
 return GenericDu3_a_AllCases.includes(value)
}
// GENERIC RECORD DU: b
export type GenericDu3_Case_CaseA_b<a,b,c> = { Case: "CaseA", item: a }
export type GenericDu3_Case_CaseB_b<a,b,c> = { Case: "CaseB" } & b
export type GenericDu3_Case_CaseC_b<a,b,c> = { Case: "CaseC", item: c }
export type GenericDu3_Case_CaseAb_b<a,b,c> = { Case: "CaseAb", item1: a, item2: b }
export type GenericDu3_b<a,b,c> = GenericDu3_Case_CaseA_b<a,b,c> | GenericDu3_Case_CaseB_b<a,b,c> | GenericDu3_Case_CaseC_b<a,b,c> | GenericDu3_Case_CaseAb_b<a,b,c>
export type GenericDu3_b_Case = "CaseA" | "CaseB" | "CaseC" | "CaseAb"
export const GenericDu3_b_AllCases = [
 "CaseA",
 "CaseB",
 "CaseC",
 "CaseAb"] satisfies GenericDu3_b_Case[]
export function isGenericDu3_b_Case(value: any): value is GenericDu3_b_Case {
 return GenericDu3_b_AllCases.includes(value)
}
// GENERIC RECORD DU: a,b
export type GenericDu3_Case_CaseA_ab<a,b,c> = { Case: "CaseA" } & a
export type GenericDu3_Case_CaseB_ab<a,b,c> = { Case: "CaseB" } & b
export type GenericDu3_Case_CaseC_ab<a,b,c> = { Case: "CaseC", item: c }
export type GenericDu3_Case_CaseAb_ab<a,b,c> = { Case: "CaseAb", item1: a, item2: b }
export type GenericDu3_ab<a,b,c> = GenericDu3_Case_CaseA_ab<a,b,c> | GenericDu3_Case_CaseB_ab<a,b,c> | GenericDu3_Case_CaseC_ab<a,b,c> | GenericDu3_Case_CaseAb_ab<a,b,c>
export type GenericDu3_ab_Case = "CaseA" | "CaseB" | "CaseC" | "CaseAb"
export const GenericDu3_ab_AllCases = [
 "CaseA",
 "CaseB",
 "CaseC",
 "CaseAb"] satisfies GenericDu3_ab_Case[]
export function isGenericDu3_ab_Case(value: any): value is GenericDu3_ab_Case {
 return GenericDu3_ab_AllCases.includes(value)
}
// GENERIC RECORD DU: c
export type GenericDu3_Case_CaseA_c<a,b,c> = { Case: "CaseA", item: a }
export type GenericDu3_Case_CaseB_c<a,b,c> = { Case: "CaseB", item: b }
export type GenericDu3_Case_CaseC_c<a,b,c> = { Case: "CaseC" } & c
export type GenericDu3_Case_CaseAb_c<a,b,c> = { Case: "CaseAb", item1: a, item2: b }
export type GenericDu3_c<a,b,c> = GenericDu3_Case_CaseA_c<a,b,c> | GenericDu3_Case_CaseB_c<a,b,c> | GenericDu3_Case_CaseC_c<a,b,c> | GenericDu3_Case_CaseAb_c<a,b,c>
export type GenericDu3_c_Case = "CaseA" | "CaseB" | "CaseC" | "CaseAb"
export const GenericDu3_c_AllCases = [
 "CaseA",
 "CaseB",
 "CaseC",
 "CaseAb"] satisfies GenericDu3_c_Case[]
export function isGenericDu3_c_Case(value: any): value is GenericDu3_c_Case {
 return GenericDu3_c_AllCases.includes(value)
}
// GENERIC RECORD DU: a,c
export type GenericDu3_Case_CaseA_ac<a,b,c> = { Case: "CaseA" } & a
export type GenericDu3_Case_CaseB_ac<a,b,c> = { Case: "CaseB", item: b }
export type GenericDu3_Case_CaseC_ac<a,b,c> = { Case: "CaseC" } & c
export type GenericDu3_Case_CaseAb_ac<a,b,c> = { Case: "CaseAb", item1: a, item2: b }
export type GenericDu3_ac<a,b,c> = GenericDu3_Case_CaseA_ac<a,b,c> | GenericDu3_Case_CaseB_ac<a,b,c> | GenericDu3_Case_CaseC_ac<a,b,c> | GenericDu3_Case_CaseAb_ac<a,b,c>
export type GenericDu3_ac_Case = "CaseA" | "CaseB" | "CaseC" | "CaseAb"
export const GenericDu3_ac_AllCases = [
 "CaseA",
 "CaseB",
 "CaseC",
 "CaseAb"] satisfies GenericDu3_ac_Case[]
export function isGenericDu3_ac_Case(value: any): value is GenericDu3_ac_Case {
 return GenericDu3_ac_AllCases.includes(value)
}
// GENERIC RECORD DU: b,c
export type GenericDu3_Case_CaseA_bc<a,b,c> = { Case: "CaseA", item: a }
export type GenericDu3_Case_CaseB_bc<a,b,c> = { Case: "CaseB" } & b
export type GenericDu3_Case_CaseC_bc<a,b,c> = { Case: "CaseC" } & c
export type GenericDu3_Case_CaseAb_bc<a,b,c> = { Case: "CaseAb", item1: a, item2: b }
export type GenericDu3_bc<a,b,c> = GenericDu3_Case_CaseA_bc<a,b,c> | GenericDu3_Case_CaseB_bc<a,b,c> | GenericDu3_Case_CaseC_bc<a,b,c> | GenericDu3_Case_CaseAb_bc<a,b,c>
export type GenericDu3_bc_Case = "CaseA" | "CaseB" | "CaseC" | "CaseAb"
export const GenericDu3_bc_AllCases = [
 "CaseA",
 "CaseB",
 "CaseC",
 "CaseAb"] satisfies GenericDu3_bc_Case[]
export function isGenericDu3_bc_Case(value: any): value is GenericDu3_bc_Case {
 return GenericDu3_bc_AllCases.includes(value)
}

"""
        )

      testCase
        "InternalTag test"
        (fun () ->
          testDuWithRecord
            (InternalTag ||| UnwrapRecordCases)
            """[{"Case":"CaseA","val":0}]"""
            """export type GenericDu_Case_CaseA<T> = { Case: "CaseA", item: T }
export type GenericDu<T> = GenericDu_Case_CaseA<T>
export type GenericDu_Case = "CaseA"
export const GenericDu_AllCases = [
 "CaseA"] satisfies GenericDu_Case[]
export function isGenericDu_Case(value: any): value is GenericDu_Case {
 return GenericDu_AllCases.includes(value)
}
// GENERIC RECORD DU: T
export type GenericDu_Case_CaseA_T<T> = { Case: "CaseA" } & T
export type GenericDu_T<T> = GenericDu_Case_CaseA_T<T>
export type GenericDu_T_Case = "CaseA"
export const GenericDu_T_AllCases = [
 "CaseA"] satisfies GenericDu_T_Case[]
export function isGenericDu_T_Case(value: any): value is GenericDu_T_Case {
 return GenericDu_T_AllCases.includes(value)
}
"""
        )

      testCase
        "ExternalTag test"
        (fun () ->
          testDuWithRecord
            (ExternalTag ||| UnwrapRecordCases)
            """[{"CaseA":{"val":0}}]"""
            """export type GenericDu_Case_CaseA<T> = { CaseA: { item: T } }
export type GenericDu<T> = GenericDu_Case_CaseA<T>
export type GenericDu_Case = "CaseA"
export const GenericDu_AllCases = [
 "CaseA"] satisfies GenericDu_Case[]
export function isGenericDu_Case(value: any): value is GenericDu_Case {
 return GenericDu_AllCases.includes(value)
}
// GENERIC RECORD DU: T
export type GenericDu_Case_CaseA_T<T> = { CaseA: T }
export type GenericDu_T<T> = GenericDu_Case_CaseA_T<T>
export type GenericDu_T_Case = "CaseA"
export const GenericDu_T_AllCases = [
 "CaseA"] satisfies GenericDu_T_Case[]
export function isGenericDu_T_Case(value: any): value is GenericDu_T_Case {
 return GenericDu_T_AllCases.includes(value)
}
"""
        )

      testCase
        "Referencing type with record - InternalTag test"
        (fun () ->
          testRefencingWithRecord
            (InternalTag ||| UnwrapRecordCases)
            """[{"val":{"Case":"CaseA","val":0}}]"""
            """export type ReferencingTypeWithRecord = {
 val: GenericDu_T<Record>
}
"""
        )

      testCase
        "Referencing type with primitive - InternalTag test"
        (fun () ->
          testRefencingWithPrimitive
            (InternalTag ||| UnwrapRecordCases)
            """[{"val":{"Case":"CaseA","item":5}}]"""
            """export type ReferencingTypeWithPrimitive = {
 val: GenericDu<System.Int32>
}
"""
        )

      testCase
        "Referencing type with union - InternalTag test"
        (fun () ->
          testRefencingWithUnion
            (InternalTag ||| UnwrapRecordCases)
            """[{"val":{"Case":"CaseA","item":{"Case":"Ok","resultValue":""}}}]"""
            """export type ReferencingTypeWithUnion = {
 val: GenericDu<Microsoft_FSharp_Core.FSharpResult<System.String,System.String>>
}
"""
        )

      testCase
        "Referencing type with record - ExternalTag test"
        (fun () ->
          testRefencingWithRecord
            (ExternalTag ||| UnwrapRecordCases)
            """[{"val":{"CaseA":{"val":0}}}]"""
            """export type ReferencingTypeWithRecord = {
 val: GenericDu_T<Record>
}
"""
        )

      testCase
        "Referencing type with primitive - ExternalTag test"
        (fun () ->
          testRefencingWithPrimitive
            (ExternalTag ||| UnwrapRecordCases)
            """[{"val":{"CaseA":{"item":5}}}]"""
            """export type ReferencingTypeWithPrimitive = {
 val: GenericDu<System.Int32>
}
"""
        )

      testCase
        "Referencing type with union - ExternalTag test"
        (fun () ->
          testRefencingWithUnion
            (ExternalTag ||| UnwrapRecordCases)
            """[{"val":{"CaseA":{"item":{"Ok":{"resultValue":""}}}}}]"""
            """export type ReferencingTypeWithUnion = {
 val: GenericDu<Microsoft_FSharp_Core.FSharpResult<System.String,System.String>>
}
"""
        )
    ]
