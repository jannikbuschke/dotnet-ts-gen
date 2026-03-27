// AdjacentTag_UnwrapFieldlessTags_UnwrapOption_UnwrapSingleFieldCases_UnwrapRecordCases
import {
  MultiCaseMultiFields,
  SingleCaseUnion,
  EnumLikeUnion,
  GenericDu,
  MyRecord,
  GenericDu0,
  GenericDu0_a,
} from './IntegrationTests';
import {
  FSharpResult,
  FSharpOption,
  FSharpResult_T,
  FSharpResult_TError,
  FSharpOption_T,
  FSharpResult_TTError,
} from './Microsoft_FSharp_Core';
import * as System from './System';

const generic0Primitive = [
  { Case: 'OneField', Fields: 25 },
] as const satisfies GenericDu0<number>[];

const generic0Record = [
  { Case: 'OneField', Fields: { val: -21 } },
] as const satisfies GenericDu0_a<MyRecord>[];

const enumLike = ['B', 'B', 'A', 'A', 'B'] as const satisfies EnumLikeUnion[];

const singleCase = [
  { Case: 'Value', Fields: 25 },
] as const satisfies SingleCaseUnion[];

const multiCase = [
  { Case: 'RecordField', Fields: { x: '\u000E9Ck' } },
  { Case: 'OneField', Fields: -25 },
  { Case: 'RecordField', Fields: { x: '\u0004\u001B\u0017/L' } },
  { Case: 'OneAnonField', Fields: { age: -18, name: '' } },
  {
    Case: 'TwoFields',
    Fields: { item1: '7|z^\u000F\u0002f\u0002PB\bI\u0016JTl;\bE', item2: true },
  },
] as const satisfies MultiCaseMultiFields[];

const generic = [
  { Case: 'OneAnonFieldAOption', Fields: { value: -45 } },
  { Case: 'OneAnonFieldAOption', Fields: { value: 29 } },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  { Case: 'OneAnonFieldAOption', Fields: { value: 19 } },
  { Case: 'TwoFields', Fields: { item1: false, item2: 0 } },
  'NoField',
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  'NoField',
  { Case: 'TwoFields', Fields: { item1: true, item2: -11 } },
  { Case: 'TwoFields', Fields: { item1: false, item2: -14 } },
  { Case: 'OneAnonFieldAOption', Fields: { value: -45 } },
  { Case: 'OneField', Fields: 41 },
  { Case: 'OneField', Fields: 39 },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
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
  { Case: 'Ok', Fields: 44 },
  { Case: 'Error', Fields: false },
  { Case: 'Ok', Fields: 19 },
  { Case: 'Error', Fields: true },
  { Case: 'Ok', Fields: 29 },
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { val: 0 },
  { val: -49 },
  { val: -3 },
  { val: 44 },
  { val: -25 },
] as const satisfies FSharpOption_T<MyRecord>[];

const resultPrimitive = [
  { Case: 'Ok', Fields: 25 },
  { Case: 'Error', Fields: true },
  { Case: 'Error', Fields: false },
] as const satisfies FSharpResult<System.Int32, System.Boolean>[];
const resultT = [
  { Case: 'Ok', Fields: { val: -21 } },
  { Case: 'Error', Fields: true },
  { Case: 'Error', Fields: false },
] as const satisfies FSharpResult_T<MyRecord, System.Boolean>[];
const resultTError = [
  { Case: 'Ok', Fields: false },
  { Case: 'Error', Fields: { val: 6 } },
  { Case: 'Error', Fields: { val: 29 } },
] as const satisfies FSharpResult_TError<System.Boolean, MyRecord>[];
const resultTTError = [
  { Case: 'Ok', Fields: { val: -21 } },
  { Case: 'Error', Fields: { val: 6 } },
  { Case: 'Error', Fields: { val: 29 } },
] as const satisfies FSharpResult_TTError<MyRecord, MyRecord>[];

// AdjacentTag_UnwrapFieldlessTags_UnwrapOption_UnwrapSingleFieldCases_UnwrapRecordCases
