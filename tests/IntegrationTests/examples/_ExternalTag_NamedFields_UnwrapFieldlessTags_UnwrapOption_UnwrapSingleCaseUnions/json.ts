// ExternalTag_NamedFields_UnwrapFieldlessTags_UnwrapOption_UnwrapSingleCaseUnions
import {
  EnumLikeUnion,
  GenericDu,
  GenericDu0,
  MultiCaseMultiFields,
  MyRecord,
  SingleCaseUnion,
} from './IntegrationTests';
import {
  FSharpOption,
  FSharpOption as FSharpOption_T,
  FSharpResult,
  FSharpResult as FSharpResult_T,
  FSharpResult as FSharpResult_TError,
  FSharpResult as FSharpResult_TTError,
} from './Microsoft_FSharp_Core';
import * as System from './System';

const generic0Primitive = [25] as const satisfies GenericDu0<number>[];

const enumLike = ['B', 'B', 'A', 'A', 'B'] as const satisfies EnumLikeUnion[];

const singleCase = [25] as const satisfies SingleCaseUnion[];

const multiCase = [
  { RecordField: { item: { x: '\u000E9Ck' } } },
  { OneField: { item: -25 } },
  { RecordField: { item: { x: '\u0004\u001B\u0017/L' } } },
  { OneAnonField: { item: { age: -18, name: '' } } },
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
  'NoField',
  { OneAnonFieldBool: { item: { value: false } } },
  { OneAnonFieldBool: { item: { value: false } } },
  'NoField',
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

// ExternalTag_NamedFields_UnwrapFieldlessTags_UnwrapOption_UnwrapSingleCaseUnions
