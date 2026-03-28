// ExternalTag_NamedFields_UnwrapFieldlessTags_UnwrapSingleFieldCases
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

const generic0Primitive = [
  { OneField: 25 },
] as const satisfies GenericDu0<number>[];

const enumLike = ['B', 'B', 'A', 'A', 'B'] as const satisfies EnumLikeUnion[];

const singleCase = [{ Value: 25 }] as const satisfies SingleCaseUnion[];

const multiCase = [
  { RecordField: { x: '\u000E9Ck' } },
  { OneField: -25 },
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
  { OneAnonFieldAOption: { value: { Some: -45 } } },
  { OneAnonFieldAOption: { value: { Some: 29 } } },
  { OneAnonFieldBool: { value: false } },
  { OneAnonFieldAOption: { value: { Some: 19 } } },
  { TwoFields: { item1: false, item2: 0 } },
  'NoField',
  { OneAnonFieldBool: { value: false } },
  { OneAnonFieldBool: { value: false } },
  'NoField',
  { TwoFields: { item1: true, item2: -11 } },
  { TwoFields: { item1: false, item2: -14 } },
  { OneAnonFieldAOption: { value: { Some: -45 } } },
  { OneField: 41 },
  { OneField: 39 },
  { OneAnonFieldBool: { value: false } },
] as const satisfies GenericDu<number, boolean>[];

const optionInt = [
  { Some: -49 },
  { Some: -47 },
] as const satisfies FSharpOption<System.Int32>[];

const optionOfAnonRecordValue = [
  { Some: { val: 0 } },
  { Some: { val: -49 } },
  { Some: { val: -3 } },
  { Some: { val: 44 } },
  { Some: { val: -25 } },
] as const satisfies FSharpOption_T<{ val: number }>[];

const optionOfResult = [
  { Some: { Ok: 44 } },
  { Some: { Error: false } },
  { Some: { Ok: 19 } },
  { Some: { Error: true } },
  { Some: { Ok: 29 } },
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { Some: { val: 0 } },
  { Some: { val: -49 } },
  { Some: { val: -3 } },
  { Some: { val: 44 } },
  { Some: { val: -25 } },
] as const satisfies FSharpOption_T<MyRecord>[];

const resultPrimitive = [
  { Ok: 25 },
  { Error: true },
  { Error: false },
] as const satisfies FSharpResult<System.Int32, System.Boolean>[];
const resultT = [
  { Ok: { val: -21 } },
  { Error: true },
  { Error: false },
] as const satisfies FSharpResult_T<MyRecord, System.Boolean>[];
const resultTError = [
  { Ok: false },
  { Error: { val: 6 } },
  { Error: { val: 29 } },
] as const satisfies FSharpResult_TError<System.Boolean, MyRecord>[];
const resultTTError = [
  { Ok: { val: -21 } },
  { Error: { val: 6 } },
  { Error: { val: 29 } },
] as const satisfies FSharpResult_TTError<MyRecord, MyRecord>[];

// ExternalTag_NamedFields_UnwrapFieldlessTags_UnwrapSingleFieldCases
