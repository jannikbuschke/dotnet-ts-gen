// InternalTag_UnwrapOption_UnwrapSingleCaseUnions
import {
  MultiCaseMultiFields,
  SingleCaseUnion,
  EnumLikeUnion,
  GenericDu,
  MyRecord,
  GenericDu0,
} from './IntegrationTests';
import {
  FSharpResult,
  FSharpOption,
  FSharpResult as FSharpResult_T,
  FSharpResult as FSharpResult_TTError,
  FSharpResult as FSharpResult_TError,
  FSharpOption as FSharpOption_T,
} from './Microsoft_FSharp_Core';
import * as System from './System';

const generic0Primitive = [25] as const satisfies GenericDu0<number>[];

const enumLike = [
  ['B'],
  ['B'],
  ['A'],
  ['A'],
  ['B'],
] as const satisfies EnumLikeUnion[];

const singleCase = [25] as const satisfies SingleCaseUnion[];

const multiCase = [
  ['RecordField', { x: '\u000E9Ck' }],
  ['OneField', -25],
  ['RecordField', { x: '\u0004\u001B\u0017/L' }],
  ['OneAnonField', { age: -18, name: '' }],
  ['TwoFields', '7|z^\u000F\u0002f\u0002PB\bI\u0016JTl;\bE', true],
] as const satisfies MultiCaseMultiFields[];

const generic = [
  ['OneAnonFieldAOption', { value: -45 }],
  ['OneAnonFieldAOption', { value: 29 }],
  ['OneAnonFieldBool', { value: false }],
  ['OneAnonFieldAOption', { value: 19 }],
  ['TwoFields', false, 0],
  ['NoField'],
  ['OneAnonFieldBool', { value: false }],
  ['OneAnonFieldBool', { value: false }],
  ['NoField'],
  ['TwoFields', true, -11],
  ['TwoFields', false, -14],
  ['OneAnonFieldAOption', { value: -45 }],
  ['OneField', 41],
  ['OneField', 39],
  ['OneAnonFieldBool', { value: false }],
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
  ['Ok', 44],
  ['Error', false],
  ['Ok', 19],
  ['Error', true],
  ['Ok', 29],
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { val: 0 },
  { val: -49 },
  { val: -3 },
  { val: 44 },
  { val: -25 },
] as const satisfies FSharpOption_T<MyRecord>[];

const resultPrimitive = [
  ['Ok', 25],
  ['Error', true],
  ['Error', false],
] as const satisfies FSharpResult<System.Int32, System.Boolean>[];
const resultT = [
  ['Ok', { val: -21 }],
  ['Error', true],
  ['Error', false],
] as const satisfies FSharpResult_T<MyRecord, System.Boolean>[];
const resultTError = [
  ['Ok', false],
  ['Error', { val: 6 }],
  ['Error', { val: 29 }],
] as const satisfies FSharpResult_TError<System.Boolean, MyRecord>[];
const resultTTError = [
  ['Ok', { val: -21 }],
  ['Error', { val: 6 }],
  ['Error', { val: 29 }],
] as const satisfies FSharpResult_TTError<MyRecord, MyRecord>[];

// InternalTag_UnwrapOption_UnwrapSingleCaseUnions
