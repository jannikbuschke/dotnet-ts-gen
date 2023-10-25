// AdjacentTag_UnwrapOption_UnwrapSingleCaseUnions_UnwrapSingleFieldCases
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

const generic0Primitive = [25] as const satisfies GenericDu0<number>[];

const enumLike = [
  { Case: 'B' },
  { Case: 'B' },
  { Case: 'A' },
  { Case: 'A' },
  { Case: 'B' },
] as const satisfies EnumLikeUnion[];

const singleCase = [25] as const satisfies SingleCaseUnion[];

const multiCase = [
  { Case: 'RecordField', Fields: {} },
  { Case: 'RecordField', Fields: {} },
  {
    Case: 'TwoFields',
    Fields: ['\u0004K\u0003\u0022\u0004\u001B\u0017/L', true],
  },
  { Case: 'Records', Fields: [{}, {}, false] },
  {
    Case: 'TwoFields',
    Fields: ['7|z^\u000F\u0002f\u0002PB\bI\u0016JTl;\bE', true],
  },
] as const satisfies MultiCaseMultiFields[];

const generic = [
  { Case: 'OneAnonFieldAOption', Fields: { value: -45 } },
  { Case: 'OneAnonFieldAOption', Fields: { value: 29 } },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  { Case: 'OneAnonFieldAOption', Fields: { value: 19 } },
  { Case: 'TwoFields', Fields: [false, 0] },
  { Case: 'NoField' },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  { Case: 'NoField' },
  { Case: 'TwoFields', Fields: [true, -11] },
  { Case: 'TwoFields', Fields: [false, -14] },
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

// AdjacentTag_UnwrapOption_UnwrapSingleCaseUnions_UnwrapSingleFieldCases
