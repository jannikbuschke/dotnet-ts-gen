// InternalTag_UnwrapOption_UnwrapRecordCases
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

const generic0Primitive = [
  { Case: 'OneField', item: 25 },
] as const satisfies GenericDu0<number>[];

const generic0Record = [
  { Case: 'OneField', val: -21 },
] as const satisfies GenericDu0_a<MyRecord>[];

const enumLike = [
  { Case: 'B' },
  { Case: 'B' },
  { Case: 'A' },
  { Case: 'A' },
  { Case: 'B' },
] as const satisfies EnumLikeUnion[];

const singleCase = [
  { Case: 'Value', item: 25 },
] as const satisfies SingleCaseUnion[];

const multiCase = [
  { Case: 'RecordField', x: '\u000E9Ck' },
  { Case: 'OneField', item: -25 },
  { Case: 'RecordField', x: '\u0004\u001B\u0017/L' },
  { Case: 'OneAnonField', age: -18, name: '' },
  {
    Case: 'TwoFields',
    item1: '7|z^\u000F\u0002f\u0002PB\bI\u0016JTl;\bE',
    item2: true,
  },
] as const satisfies MultiCaseMultiFields[];

const generic = [
  { Case: 'OneAnonFieldAOption', value: -45 },
  { Case: 'OneAnonFieldAOption', value: 29 },
  { Case: 'OneAnonFieldBool', value: false },
  { Case: 'OneAnonFieldAOption', value: 19 },
  { Case: 'TwoFields', item1: false, item2: 0 },
  { Case: 'NoField' },
  { Case: 'OneAnonFieldBool', value: false },
  { Case: 'OneAnonFieldBool', value: false },
  { Case: 'NoField' },
  { Case: 'TwoFields', item1: true, item2: -11 },
  { Case: 'TwoFields', item1: false, item2: -14 },
  { Case: 'OneAnonFieldAOption', value: -45 },
  { Case: 'OneField', item: 41 },
  { Case: 'OneField', item: 39 },
  { Case: 'OneAnonFieldBool', value: false },
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
  { Case: 'Ok', resultValue: 44 },
  { Case: 'Error', errorValue: false },
  { Case: 'Ok', resultValue: 19 },
  { Case: 'Error', errorValue: true },
  { Case: 'Ok', resultValue: 29 },
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { val: 0 },
  { val: -49 },
  { val: -3 },
  { val: 44 },
  { val: -25 },
] as const satisfies FSharpOption_T<MyRecord>[];

const resultPrimitive = [
  { Case: 'Ok', resultValue: 25 },
  { Case: 'Error', errorValue: true },
  { Case: 'Error', errorValue: false },
] as const satisfies FSharpResult<System.Int32, System.Boolean>[];
const resultT = [
  { Case: 'Ok', val: -21 },
  { Case: 'Error', errorValue: true },
  { Case: 'Error', errorValue: false },
] as const satisfies FSharpResult_T<MyRecord, System.Boolean>[];
const resultTError = [
  { Case: 'Ok', resultValue: false },
  { Case: 'Error', val: 6 },
  { Case: 'Error', val: 29 },
] as const satisfies FSharpResult_TError<System.Boolean, MyRecord>[];
const resultTTError = [
  { Case: 'Ok', val: -21 },
  { Case: 'Error', val: 6 },
  { Case: 'Error', val: 29 },
] as const satisfies FSharpResult_TTError<MyRecord, MyRecord>[];

// InternalTag_UnwrapOption_UnwrapRecordCases
