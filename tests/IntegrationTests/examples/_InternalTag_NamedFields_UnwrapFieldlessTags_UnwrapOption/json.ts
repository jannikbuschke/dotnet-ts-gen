// InternalTag_NamedFields_UnwrapFieldlessTags_UnwrapOption
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

const generic0Primitive = [
  { Case: 'OneField', item: 25 },
] as const satisfies GenericDu0<number>[];

const enumLike = ['B', 'B', 'A', 'A', 'B'] as const satisfies EnumLikeUnion[];

const singleCase = [
  { Case: 'Value', item: 25 },
] as const satisfies SingleCaseUnion[];

const multiCase = [
  { Case: 'RecordField', item: { x: '\u000E9Ck' } },
  { Case: 'OneField', item: -25 },
  { Case: 'RecordField', item: { x: '\u0004\u001B\u0017/L' } },
  { Case: 'OneAnonField', item: { age: -18, name: '' } },
  {
    Case: 'TwoFields',
    item1: '7|z^\u000F\u0002f\u0002PB\bI\u0016JTl;\bE',
    item2: true,
  },
] as const satisfies MultiCaseMultiFields[];

const generic = [
  { Case: 'OneAnonFieldAOption', item: { value: -45 } },
  { Case: 'OneAnonFieldAOption', item: { value: 29 } },
  { Case: 'OneAnonFieldBool', item: { value: false } },
  { Case: 'OneAnonFieldAOption', item: { value: 19 } },
  { Case: 'TwoFields', item1: false, item2: 0 },
  'NoField',
  { Case: 'OneAnonFieldBool', item: { value: false } },
  { Case: 'OneAnonFieldBool', item: { value: false } },
  'NoField',
  { Case: 'TwoFields', item1: true, item2: -11 },
  { Case: 'TwoFields', item1: false, item2: -14 },
  { Case: 'OneAnonFieldAOption', item: { value: -45 } },
  { Case: 'OneField', item: 41 },
  { Case: 'OneField', item: 39 },
  { Case: 'OneAnonFieldBool', item: { value: false } },
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
  { Case: 'Ok', resultValue: { val: -21 } },
  { Case: 'Error', errorValue: true },
  { Case: 'Error', errorValue: false },
] as const satisfies FSharpResult_T<MyRecord, System.Boolean>[];
const resultTError = [
  { Case: 'Ok', resultValue: false },
  { Case: 'Error', errorValue: { val: 6 } },
  { Case: 'Error', errorValue: { val: 29 } },
] as const satisfies FSharpResult_TError<System.Boolean, MyRecord>[];
const resultTTError = [
  { Case: 'Ok', resultValue: { val: -21 } },
  { Case: 'Error', errorValue: { val: 6 } },
  { Case: 'Error', errorValue: { val: 29 } },
] as const satisfies FSharpResult_TTError<MyRecord, MyRecord>[];

// InternalTag_NamedFields_UnwrapFieldlessTags_UnwrapOption
