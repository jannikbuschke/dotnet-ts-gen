// AdjacentTag_NamedFields_UnwrapOption
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
  { Case: 'OneField', Fields: { item: 25 } },
] as const satisfies GenericDu0<number>[];

const enumLike = [
  { Case: 'B' },
  { Case: 'B' },
  { Case: 'A' },
  { Case: 'A' },
  { Case: 'B' },
] as const satisfies EnumLikeUnion[];

const singleCase = [
  { Case: 'Value', Fields: { item: 25 } },
] as const satisfies SingleCaseUnion[];

const multiCase = [
  { Case: 'RecordField', Fields: { item: { x: '\u000E9Ck' } } },
  { Case: 'OneField', Fields: { item: -25 } },
  { Case: 'RecordField', Fields: { item: { x: '\u0004\u001B\u0017/L' } } },
  { Case: 'OneAnonField', Fields: { item: { age: -18, name: '' } } },
  {
    Case: 'TwoFields',
    Fields: { item1: '7|z^\u000F\u0002f\u0002PB\bI\u0016JTl;\bE', item2: true },
  },
] as const satisfies MultiCaseMultiFields[];

const generic = [
  { Case: 'OneAnonFieldAOption', Fields: { item: { value: -45 } } },
  { Case: 'OneAnonFieldAOption', Fields: { item: { value: 29 } } },
  { Case: 'OneAnonFieldBool', Fields: { item: { value: false } } },
  { Case: 'OneAnonFieldAOption', Fields: { item: { value: 19 } } },
  { Case: 'TwoFields', Fields: { item1: false, item2: 0 } },
  { Case: 'NoField' },
  { Case: 'OneAnonFieldBool', Fields: { item: { value: false } } },
  { Case: 'OneAnonFieldBool', Fields: { item: { value: false } } },
  { Case: 'NoField' },
  { Case: 'TwoFields', Fields: { item1: true, item2: -11 } },
  { Case: 'TwoFields', Fields: { item1: false, item2: -14 } },
  { Case: 'OneAnonFieldAOption', Fields: { item: { value: -45 } } },
  { Case: 'OneField', Fields: { item: 41 } },
  { Case: 'OneField', Fields: { item: 39 } },
  { Case: 'OneAnonFieldBool', Fields: { item: { value: false } } },
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
  { Case: 'Ok', Fields: { resultValue: 44 } },
  { Case: 'Error', Fields: { errorValue: false } },
  { Case: 'Ok', Fields: { resultValue: 19 } },
  { Case: 'Error', Fields: { errorValue: true } },
  { Case: 'Ok', Fields: { resultValue: 29 } },
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { val: 0 },
  { val: -49 },
  { val: -3 },
  { val: 44 },
  { val: -25 },
] as const satisfies FSharpOption_T<MyRecord>[];

const resultPrimitive = [
  { Case: 'Ok', Fields: { resultValue: 25 } },
  { Case: 'Error', Fields: { errorValue: true } },
  { Case: 'Error', Fields: { errorValue: false } },
] as const satisfies FSharpResult<System.Int32, System.Boolean>[];
const resultT = [
  { Case: 'Ok', Fields: { resultValue: { val: -21 } } },
  { Case: 'Error', Fields: { errorValue: true } },
  { Case: 'Error', Fields: { errorValue: false } },
] as const satisfies FSharpResult_T<MyRecord, System.Boolean>[];
const resultTError = [
  { Case: 'Ok', Fields: { resultValue: false } },
  { Case: 'Error', Fields: { errorValue: { val: 6 } } },
  { Case: 'Error', Fields: { errorValue: { val: 29 } } },
] as const satisfies FSharpResult_TError<System.Boolean, MyRecord>[];
const resultTTError = [
  { Case: 'Ok', Fields: { resultValue: { val: -21 } } },
  { Case: 'Error', Fields: { errorValue: { val: 6 } } },
  { Case: 'Error', Fields: { errorValue: { val: 29 } } },
] as const satisfies FSharpResult_TTError<MyRecord, MyRecord>[];

// AdjacentTag_NamedFields_UnwrapOption
