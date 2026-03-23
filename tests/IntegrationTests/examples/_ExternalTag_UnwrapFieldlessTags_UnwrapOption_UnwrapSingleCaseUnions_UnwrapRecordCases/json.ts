// ExternalTag_UnwrapFieldlessTags_UnwrapOption_UnwrapSingleCaseUnions_UnwrapRecordCases
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

const enumLike = ['B', 'B', 'A', 'A', 'B'] as const satisfies EnumLikeUnion[];

const singleCase = [25] as const satisfies SingleCaseUnion[];

const multiCase = [
  { RecordField: { foo: -21 } },
  {
    TwoFields: {
      item1: '\u0004K\u0003\u0022\u0004\u001B\u0017/L',
      item2: true,
    },
  },
  { Records: { item1: { foo: -2 }, item2: { foo: 14 }, item3: true } },
  { OneField: { item: -1 } },
  'NoField',
] as const satisfies MultiCaseMultiFields[];

const generic = [
  { OneAnonFieldAOption: { value: -45 } },
  { OneAnonFieldAOption: { value: 29 } },
  { OneAnonFieldBool: { value: false } },
  { OneAnonFieldAOption: { value: 19 } },
  { TwoFields: { item1: false, item2: 0 } },
  'NoField',
  { OneAnonFieldBool: { value: false } },
  { OneAnonFieldBool: { value: false } },
  'NoField',
  { TwoFields: { item1: true, item2: -11 } },
  { TwoFields: { item1: false, item2: -14 } },
  { OneAnonFieldAOption: { value: -45 } },
  { OneField: { item: 41 } },
  { OneField: { item: 39 } },
  { OneAnonFieldBool: { value: false } },
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
  { Ok: { resultValue: 44 } },
  { Error: { errorValue: false } },
  { Ok: { resultValue: 19 } },
  { Error: { errorValue: true } },
  { Ok: { resultValue: 29 } },
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { val: 0 },
  { val: -49 },
  { val: -3 },
  { val: 44 },
  { val: -25 },
] as const satisfies FSharpOption_T<MyRecord>[];

const resultPrimitive = [
  { Ok: { resultValue: 25 } },
  { Error: { errorValue: true } },
  { Error: { errorValue: false } },
] as const satisfies FSharpResult<System.Int32, System.Boolean>[];
const resultT = [
  { Ok: { val: -21 } },
  { Error: { errorValue: true } },
  { Error: { errorValue: false } },
] as const satisfies FSharpResult_T<MyRecord, System.Boolean>[];
const resultTError = [
  { Ok: { resultValue: false } },
  { Error: { val: 6 } },
  { Error: { val: 29 } },
] as const satisfies FSharpResult_TError<System.Boolean, MyRecord>[];
const resultTTError = [
  { Ok: { val: -21 } },
  { Error: { val: 6 } },
  { Error: { val: 29 } },
] as const satisfies FSharpResult_TTError<MyRecord, MyRecord>[];

// ExternalTag_UnwrapFieldlessTags_UnwrapOption_UnwrapSingleCaseUnions_UnwrapRecordCases
