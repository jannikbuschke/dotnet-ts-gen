// InternalTag_UnwrapSingleCaseUnions_UnwrapSingleFieldCases_UnwrapRecordCases
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
  { Case: 'RecordField', foo: -21 },
  {
    Case: 'TwoFields',
    item1: '\u0004K\u0003\u0022\u0004\u001B\u0017/L',
    item2: true,
  },
  { Case: 'Records', item1: { foo: -2 }, item2: { foo: 14 }, item3: true },
  { Case: 'OneField', item: -1 },
  { Case: 'NoField' },
] as const satisfies MultiCaseMultiFields[];

const generic = [
  { Case: 'OneAnonFieldAOption', value: { Case: 'Some', value: -45 } },
  { Case: 'OneAnonFieldAOption', value: { Case: 'Some', value: 29 } },
  { Case: 'OneAnonFieldBool', value: false },
  { Case: 'OneAnonFieldAOption', value: { Case: 'Some', value: 19 } },
  { Case: 'TwoFields', item1: false, item2: 0 },
  { Case: 'NoField' },
  { Case: 'OneAnonFieldBool', value: false },
  { Case: 'OneAnonFieldBool', value: false },
  { Case: 'NoField' },
  { Case: 'TwoFields', item1: true, item2: -11 },
  { Case: 'TwoFields', item1: false, item2: -14 },
  { Case: 'OneAnonFieldAOption', value: { Case: 'Some', value: -45 } },
  { Case: 'OneField', item: 41 },
  { Case: 'OneField', item: 39 },
  { Case: 'OneAnonFieldBool', value: false },
] as const satisfies GenericDu<number, boolean>[];

const optionInt = [
  { Case: 'Some', value: -49 },
  { Case: 'Some', value: -47 },
] as const satisfies FSharpOption<System.Int32>[];

const optionOfAnonRecordValue = [
  { Case: 'Some', val: 0 },
  { Case: 'Some', val: -49 },
  { Case: 'Some', val: -3 },
  { Case: 'Some', val: 44 },
  { Case: 'Some', val: -25 },
] as const satisfies FSharpOption_T<{ val: number }>[];

const optionOfResult = [
  { Case: 'Some', value: { Case: 'Ok', resultValue: 44 } },
  { Case: 'Some', value: { Case: 'Error', errorValue: false } },
  { Case: 'Some', value: { Case: 'Ok', resultValue: 19 } },
  { Case: 'Some', value: { Case: 'Error', errorValue: true } },
  { Case: 'Some', value: { Case: 'Ok', resultValue: 29 } },
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { Case: 'Some', val: 0 },
  { Case: 'Some', val: -49 },
  { Case: 'Some', val: -3 },
  { Case: 'Some', val: 44 },
  { Case: 'Some', val: -25 },
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

// InternalTag_UnwrapSingleCaseUnions_UnwrapSingleFieldCases_UnwrapRecordCases
