// AdjacentTag_NamedFields_UnwrapSingleFieldCases
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
  { Case: 'OneField', Fields: 25 },
] as const satisfies GenericDu0<number>[];

const enumLike = [
  { Case: 'B' },
  { Case: 'B' },
  { Case: 'A' },
  { Case: 'A' },
  { Case: 'B' },
] as const satisfies EnumLikeUnion[];

const singleCase = [
  { Case: 'Value', Fields: 25 },
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
  { Case: 'OneField', Fields: -1 },
  { Case: 'NoField' },
] as const satisfies MultiCaseMultiFields[];

const generic = [
  {
    Case: 'OneAnonFieldAOption',
    Fields: { value: { Case: 'Some', Fields: -45 } },
  },
  {
    Case: 'OneAnonFieldAOption',
    Fields: { value: { Case: 'Some', Fields: 29 } },
  },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
  {
    Case: 'OneAnonFieldAOption',
    Fields: { value: { Case: 'Some', Fields: 19 } },
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
    Fields: { value: { Case: 'Some', Fields: -45 } },
  },
  { Case: 'OneField', Fields: 41 },
  { Case: 'OneField', Fields: 39 },
  { Case: 'OneAnonFieldBool', Fields: { value: false } },
] as const satisfies GenericDu<number, boolean>[];

const optionInt = [
  { Case: 'Some', Fields: -49 },
  { Case: 'Some', Fields: -47 },
] as const satisfies FSharpOption<System.Int32>[];

const optionOfAnonRecordValue = [
  { Case: 'Some', Fields: { val: 0 } },
  { Case: 'Some', Fields: { val: -49 } },
  { Case: 'Some', Fields: { val: -3 } },
  { Case: 'Some', Fields: { val: 44 } },
  { Case: 'Some', Fields: { val: -25 } },
] as const satisfies FSharpOption_T<{ val: number }>[];

const optionOfResult = [
  { Case: 'Some', Fields: { Case: 'Ok', Fields: 44 } },
  { Case: 'Some', Fields: { Case: 'Error', Fields: false } },
  { Case: 'Some', Fields: { Case: 'Ok', Fields: 19 } },
  { Case: 'Some', Fields: { Case: 'Error', Fields: true } },
  { Case: 'Some', Fields: { Case: 'Ok', Fields: 29 } },
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { Case: 'Some', Fields: { val: 0 } },
  { Case: 'Some', Fields: { val: -49 } },
  { Case: 'Some', Fields: { val: -3 } },
  { Case: 'Some', Fields: { val: 44 } },
  { Case: 'Some', Fields: { val: -25 } },
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

// AdjacentTag_NamedFields_UnwrapSingleFieldCases
