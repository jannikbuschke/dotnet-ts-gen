// AdjacentTag_UnwrapSingleCaseUnions_UnwrapRecordCases
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

const enumLike = [
  { Case: 'B' },
  { Case: 'B' },
  { Case: 'A' },
  { Case: 'A' },
  { Case: 'B' },
] as const satisfies EnumLikeUnion[];

const singleCase = [25] as const satisfies SingleCaseUnion[];

const multiCase = [
  { Case: 'RecordField', Fields: { x: '\u000E9Ck' } },
  { Case: 'OneField', Fields: { item: -25 } },
  { Case: 'RecordField', Fields: { x: '\u0004\u001B\u0017/L' } },
  { Case: 'OneAnonField', Fields: { age: -18, name: '' } },
  {
    Case: 'TwoFields',
    Fields: { item1: '7|z^\u000F\u0002f\u0002PB\bI\u0016JTl;\bE', item2: true },
  },
] as const satisfies MultiCaseMultiFields[];

const generic = [
  {
    Case: 'OneAnonFieldAOption',
    Fields: { value: { Case: 'Some', Fields: { value: -45 } } },
  },
  {
    Case: 'OneAnonFieldAOption',
    Fields: { value: { Case: 'Some', Fields: { value: 29 } } },
  },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  {
    Case: 'OneAnonFieldAOption',
    Fields: { value: { Case: 'Some', Fields: { value: 19 } } },
  },
  { Case: 'TwoFields', Fields: { item1: false, item2: 0 } },
  { Case: 'NoField' },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  { Case: 'NoField' },
  { Case: 'TwoFields', Fields: { item1: true, item2: -11 } },
  { Case: 'TwoFields', Fields: { item1: false, item2: -14 } },
  {
    Case: 'OneAnonFieldAOption',
    Fields: { value: { Case: 'Some', Fields: { value: -45 } } },
  },
  { Case: 'OneField', Fields: { item: 41 } },
  { Case: 'OneField', Fields: { item: 39 } },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
] as const satisfies GenericDu<number, boolean>[];

const optionInt = [
  { Case: 'Some', Fields: { value: -49 } },
  { Case: 'Some', Fields: { value: -47 } },
] as const satisfies FSharpOption<System.Int32>[];

const optionOfAnonRecordValue = [
  { Case: 'Some', Fields: { val: 0 } },
  { Case: 'Some', Fields: { val: -49 } },
  { Case: 'Some', Fields: { val: -3 } },
  { Case: 'Some', Fields: { val: 44 } },
  { Case: 'Some', Fields: { val: -25 } },
] as const satisfies FSharpOption_T<{ val: number }>[];

const optionOfResult = [
  {
    Case: 'Some',
    Fields: { value: { Case: 'Ok', Fields: { resultValue: 44 } } },
  },
  {
    Case: 'Some',
    Fields: { value: { Case: 'Error', Fields: { errorValue: false } } },
  },
  {
    Case: 'Some',
    Fields: { value: { Case: 'Ok', Fields: { resultValue: 19 } } },
  },
  {
    Case: 'Some',
    Fields: { value: { Case: 'Error', Fields: { errorValue: true } } },
  },
  {
    Case: 'Some',
    Fields: { value: { Case: 'Ok', Fields: { resultValue: 29 } } },
  },
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { Case: 'Some', Fields: { val: 0 } },
  { Case: 'Some', Fields: { val: -49 } },
  { Case: 'Some', Fields: { val: -3 } },
  { Case: 'Some', Fields: { val: 44 } },
  { Case: 'Some', Fields: { val: -25 } },
] as const satisfies FSharpOption_T<MyRecord>[];

const resultPrimitive = [
  { Case: 'Ok', Fields: { resultValue: 25 } },
  { Case: 'Error', Fields: { errorValue: true } },
  { Case: 'Error', Fields: { errorValue: false } },
] as const satisfies FSharpResult<System.Int32, System.Boolean>[];
const resultT = [
  { Case: 'Ok', Fields: { val: -21 } },
  { Case: 'Error', Fields: { errorValue: true } },
  { Case: 'Error', Fields: { errorValue: false } },
] as const satisfies FSharpResult_T<MyRecord, System.Boolean>[];
const resultTError = [
  { Case: 'Ok', Fields: { resultValue: false } },
  { Case: 'Error', Fields: { val: 6 } },
  { Case: 'Error', Fields: { val: 29 } },
] as const satisfies FSharpResult_TError<System.Boolean, MyRecord>[];
const resultTTError = [
  { Case: 'Ok', Fields: { val: -21 } },
  { Case: 'Error', Fields: { val: 6 } },
  { Case: 'Error', Fields: { val: 29 } },
] as const satisfies FSharpResult_TTError<MyRecord, MyRecord>[];

// AdjacentTag_UnwrapSingleCaseUnions_UnwrapRecordCases
