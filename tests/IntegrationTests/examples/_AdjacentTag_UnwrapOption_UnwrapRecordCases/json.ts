// AdjacentTag_UnwrapOption_UnwrapRecordCases
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
  { Case: 'OneField', Fields: { item: 25 } },
] as const satisfies GenericDu0<number>[];

const generic0Record = [
  { Case: 'OneField', Fields: { val: -21 } },
] as const satisfies GenericDu0_a<MyRecord>[];

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
  { Case: 'RecordField', Fields: { foo: -21 } },
  {
    Case: 'TwoFields',
    Fields: { item1: '\u0004K\u0003\u0022\u0004\u001B\u0017/L', item2: true },
  },
  {
    Case: 'Records',
    Fields: { item1: { foo: -2 }, item2: { foo: 14 }, item3: true },
  },
  { Case: 'OneField', Fields: { item: -1 } },
  { Case: 'NoField' },
] as const satisfies MultiCaseMultiFields[];

const generic = [
  { Case: 'OneAnonFieldAOption', Fields: { value: -45 } },
  { Case: 'OneAnonFieldAOption', Fields: { value: 29 } },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  { Case: 'OneAnonFieldAOption', Fields: { value: 19 } },
  { Case: 'TwoFields', Fields: { item1: false, item2: 0 } },
  { Case: 'NoField' },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  { Case: 'NoField' },
  { Case: 'TwoFields', Fields: { item1: true, item2: -11 } },
  { Case: 'TwoFields', Fields: { item1: false, item2: -14 } },
  { Case: 'OneAnonFieldAOption', Fields: { value: -45 } },
  { Case: 'OneField', Fields: { item: 41 } },
  { Case: 'OneField', Fields: { item: 39 } },
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

// AdjacentTag_UnwrapOption_UnwrapRecordCases
