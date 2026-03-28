// ExternalTag_UnwrapFieldlessTags_UnwrapSingleCaseUnions_UnwrapRecordCases
import {
  EnumLikeUnion,
  GenericDu,
  GenericDu0,
  GenericDu0_a,
  MultiCaseMultiFields,
  MyRecord,
  SingleCaseUnion,
} from './IntegrationTests';
import {
  FSharpOption,
  FSharpOption_T,
  FSharpResult,
  FSharpResult_T,
  FSharpResult_TError,
  FSharpResult_TTError,
} from './Microsoft_FSharp_Core';
import * as System from './System';

const generic0Primitive = [25] as const satisfies GenericDu0<number>[];

const generic0Record = [
  { val: -21 },
] as const satisfies GenericDu0_a<MyRecord>[];

const enumLike = ['B', 'B', 'A', 'A', 'B'] as const satisfies EnumLikeUnion[];

const singleCase = [25] as const satisfies SingleCaseUnion[];

const multiCase = [
  { RecordField: { x: '\u000E9Ck' } },
  { OneField: { item: -25 } },
  { RecordField: { x: '\u0004\u001B\u0017/L' } },
  { OneAnonField: { age: -18, name: '' } },
  {
    TwoFields: {
      item1: '7|z^\u000F\u0002f\u0002PB\bI\u0016JTl;\bE',
      item2: true,
    },
  },
] as const satisfies MultiCaseMultiFields[];

const generic = [
  { OneAnonFieldAOption: { value: { Some: { value: -45 } } } },
  { OneAnonFieldAOption: { value: { Some: { value: 29 } } } },
  { OneAnonFieldBool: { value: false } },
  { OneAnonFieldAOption: { value: { Some: { value: 19 } } } },
  { TwoFields: { item1: false, item2: 0 } },
  'NoField',
  { OneAnonFieldBool: { value: false } },
  { OneAnonFieldBool: { value: false } },
  'NoField',
  { TwoFields: { item1: true, item2: -11 } },
  { TwoFields: { item1: false, item2: -14 } },
  { OneAnonFieldAOption: { value: { Some: { value: -45 } } } },
  { OneField: { item: 41 } },
  { OneField: { item: 39 } },
  { OneAnonFieldBool: { value: false } },
] as const satisfies GenericDu<number, boolean>[];

const optionInt = [
  { Some: { value: -49 } },
  { Some: { value: -47 } },
] as const satisfies FSharpOption<System.Int32>[];

const optionOfAnonRecordValue = [
  { Some: { val: 0 } },
  { Some: { val: -49 } },
  { Some: { val: -3 } },
  { Some: { val: 44 } },
  { Some: { val: -25 } },
] as const satisfies FSharpOption_T<{ val: number }>[];

const optionOfResult = [
  { Some: { value: { Ok: { resultValue: 44 } } } },
  { Some: { value: { Error: { errorValue: false } } } },
  { Some: { value: { Ok: { resultValue: 19 } } } },
  { Some: { value: { Error: { errorValue: true } } } },
  { Some: { value: { Ok: { resultValue: 29 } } } },
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { Some: { val: 0 } },
  { Some: { val: -49 } },
  { Some: { val: -3 } },
  { Some: { val: 44 } },
  { Some: { val: -25 } },
] as const satisfies FSharpOption_T<MyRecord>[];

const resultPrimitive = [
  { Ok: { resultValue: 25 } },
  { Error: { errorValue: true } },
  { Error: { errorValue: false } },
] as const satisfies FSharpResult<System.Int32, System.Boolean>[];
const resultT = [
  { Ok: { val: -21 } },
  { Error: { errorValue: true } },
  { Error: { errorValue: false } },
] as const satisfies FSharpResult_T<MyRecord, System.Boolean>[];
const resultTError = [
  { Ok: { resultValue: false } },
  { Error: { val: 6 } },
  { Error: { val: 29 } },
] as const satisfies FSharpResult_TError<System.Boolean, MyRecord>[];
const resultTTError = [
  { Ok: { val: -21 } },
  { Error: { val: 6 } },
  { Error: { val: 29 } },
] as const satisfies FSharpResult_TTError<MyRecord, MyRecord>[];

// ExternalTag_UnwrapFieldlessTags_UnwrapSingleCaseUnions_UnwrapRecordCases
