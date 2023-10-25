// ExternalTag_NamedFields_UnwrapOption
import {
  MultiCaseMultiFields,
  SingleCaseUnion,
  EnumLikeUnion,
  GenericDu,
  MyRecord,
  GenericDu0,
} from './IntegrationTests_Gen';
import {
  FSharpResult,
  FSharpOption,
  FSharpResult as FSharpResult_T,
  FSharpResult as FSharpResult_TTError,
  FSharpResult as FSharpResult_TError,
  FSharpOption as FSharpOption_T,
} from './Microsoft_FSharp_Core';
import * as System from './System';

const generic0Primitive = [
  { OneField: { item: 25 } },
] as const satisfies GenericDu0<number>[];

const enumLike = [
  { B: {} },
  { B: {} },
  { A: {} },
  { A: {} },
  { B: {} },
] as const satisfies EnumLikeUnion[];

const singleCase = [
  { Value: { item: 25 } },
] as const satisfies SingleCaseUnion[];

const multiCase = [
  { RecordField: { item: {} } },
  { RecordField: { item: {} } },
  {
    TwoFields: {
      item1: '\u0004K\u0003\u0022\u0004\u001B\u0017/L',
      item2: true,
    },
  },
  { Records: { item1: {}, item2: {}, item3: false } },
  {
    TwoFields: {
      item1: '7|z^\u000F\u0002f\u0002PB\bI\u0016JTl;\bE',
      item2: true,
    },
  },
] as const satisfies MultiCaseMultiFields[];

const generic = [
  { OneAnonFieldAOption: { item: { value: -45 } } },
  { OneAnonFieldAOption: { item: { value: 29 } } },
  { OneAnonFieldBool: { item: { value: false } } },
  { OneAnonFieldAOption: { item: { value: 19 } } },
  { TwoFields: { item1: false, item2: 0 } },
  { NoField: {} },
  { OneAnonFieldBool: { item: { value: false } } },
  { OneAnonFieldBool: { item: { value: false } } },
  { NoField: {} },
  { TwoFields: { item1: true, item2: -11 } },
  { TwoFields: { item1: false, item2: -14 } },
  { OneAnonFieldAOption: { item: { value: -45 } } },
  { OneField: { item: 41 } },
  { OneField: { item: 39 } },
  { OneAnonFieldBool: { item: { value: false } } },
] as const satisfies GenericDu<number, boolean>[];

const optionInt = [-49, -47] as const satisfies FSharpOption<System.Int32>[];

const optionOfAnonRecordValue = [
  { val: 0 },
  { val: -49 },
  { val: -3 },
  { val: 44 },
  { val: -25 },
] as const satisfies FSharpOption_T<{ val: number }>[];

const optionOfResult = [
  { Ok: { resultValue: 44 } },
  { Error: { errorValue: false } },
  { Ok: { resultValue: 19 } },
  { Error: { errorValue: true } },
  { Ok: { resultValue: 29 } },
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { val: 0 },
  { val: -49 },
  { val: -3 },
  { val: 44 },
  { val: -25 },
] as const satisfies FSharpOption_T<MyRecord>[];

const resultPrimitive = [
  { Ok: { resultValue: 25 } },
  { Error: { errorValue: true } },
  { Error: { errorValue: false } },
] as const satisfies FSharpResult<System.Int32, System.Boolean>[];
const resultT = [
  { Ok: { resultValue: { val: -21 } } },
  { Error: { errorValue: true } },
  { Error: { errorValue: false } },
] as const satisfies FSharpResult_T<MyRecord, System.Boolean>[];
const resultTError = [
  { Ok: { resultValue: false } },
  { Error: { errorValue: { val: 6 } } },
  { Error: { errorValue: { val: 29 } } },
] as const satisfies FSharpResult_TError<System.Boolean, MyRecord>[];
const resultTTError = [
  { Ok: { resultValue: { val: -21 } } },
  { Error: { errorValue: { val: 6 } } },
  { Error: { errorValue: { val: 29 } } },
] as const satisfies FSharpResult_TTError<MyRecord, MyRecord>[];

// ExternalTag_NamedFields_UnwrapOption
