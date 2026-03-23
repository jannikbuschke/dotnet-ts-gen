// AdjacentTag_NamedFields_UnwrapFieldlessTags_UnwrapOption_UnwrapSingleCaseUnions
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

const enumLike = ['B', 'B', 'A', 'A', 'B'] as const satisfies EnumLikeUnion[];

const singleCase = [25] as const satisfies SingleCaseUnion[];

const multiCase = [
  { Case: 'RecordField', Fields: { item: { foo: -21 } } },
  {
    Case: 'TwoFields',
    Fields: { item1: '\u0004K\u0003\u0022\u0004\u001B\u0017/L', item2: true },
  },
  {
    Case: 'Records',
    Fields: { item1: { foo: -2 }, item2: { foo: 14 }, item3: true },
  },
  { Case: 'OneField', Fields: { item: -1 } },
  'NoField',
] as const satisfies MultiCaseMultiFields[];

const generic = [
  { Case: 'OneAnonFieldAOption', Fields: { item: { value: -45 } } },
  { Case: 'OneAnonFieldAOption', Fields: { item: { value: 29 } } },
  { Case: 'OneAnonFieldBool', Fields: { item: { value: false } } },
  { Case: 'OneAnonFieldAOption', Fields: { item: { value: 19 } } },
  { Case: 'TwoFields', Fields: { item1: false, item2: 0 } },
  'NoField',
  { Case: 'OneAnonFieldBool', Fields: { item: { value: false } } },
  { Case: 'OneAnonFieldBool', Fields: { item: { value: false } } },
  'NoField',
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

// AdjacentTag_NamedFields_UnwrapFieldlessTags_UnwrapOption_UnwrapSingleCaseUnions
