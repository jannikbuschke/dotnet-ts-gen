// ExternalTag_NamedFields_UnwrapFieldlessTags
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
  { OneField: { item: 25 } },
] as const satisfies GenericDu0<number>[];

const enumLike = ['B', 'B', 'A', 'A', 'B'] as const satisfies EnumLikeUnion[];

const singleCase = [
  { Value: { item: 25 } },
] as const satisfies SingleCaseUnion[];

const multiCase = [
  { RecordField: { item: { foo: -21 } } },
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
  { OneAnonFieldAOption: { item: { value: { Some: { value: -45 } } } } },
  { OneAnonFieldAOption: { item: { value: { Some: { value: 29 } } } } },
  { OneAnonFieldBool: { item: { value: false } } },
  { OneAnonFieldAOption: { item: { value: { Some: { value: 19 } } } } },
  { TwoFields: { item1: false, item2: 0 } },
  'NoField',
  { OneAnonFieldBool: { item: { value: false } } },
  { OneAnonFieldBool: { item: { value: false } } },
  'NoField',
  { TwoFields: { item1: true, item2: -11 } },
  { TwoFields: { item1: false, item2: -14 } },
  { OneAnonFieldAOption: { item: { value: { Some: { value: -45 } } } } },
  { OneField: { item: 41 } },
  { OneField: { item: 39 } },
  { OneAnonFieldBool: { item: { value: false } } },
] as const satisfies GenericDu<number, boolean>[];

const optionInt = [
  { Some: { value: -49 } },
  { Some: { value: -47 } },
] as const satisfies FSharpOption<System.Int32>[];

const optionOfAnonRecordValue = [
  { Some: { value: { val: 0 } } },
  { Some: { value: { val: -49 } } },
  { Some: { value: { val: -3 } } },
  { Some: { value: { val: 44 } } },
  { Some: { value: { val: -25 } } },
] as const satisfies FSharpOption_T<{ val: number }>[];

const optionOfResult = [
  { Some: { value: { Ok: { resultValue: 44 } } } },
  { Some: { value: { Error: { errorValue: false } } } },
  { Some: { value: { Ok: { resultValue: 19 } } } },
  { Some: { value: { Error: { errorValue: true } } } },
  { Some: { value: { Ok: { resultValue: 29 } } } },
] as const satisfies FSharpOption<FSharpResult<System.Int32, System.Boolean>>[];

const optionOfRecord = [
  { Some: { value: { val: 0 } } },
  { Some: { value: { val: -49 } } },
  { Some: { value: { val: -3 } } },
  { Some: { value: { val: 44 } } },
  { Some: { value: { val: -25 } } },
] as const satisfies FSharpOption_T<MyRecord>[];

const resultPrimitive = [
  { Ok: { resultValue: 25 } },
  { Error: { errorValue: true } },
  { Error: { errorValue: false } },
] as const satisfies FSharpResult<System.Int32, System.Boolean>[];
const resultT = [
  { Ok: { resultValue: { val: -21 } } },
  { Error: { errorValue: true } },
  { Error: { errorValue: false } },
] as const satisfies FSharpResult_T<MyRecord, System.Boolean>[];
const resultTError = [
  { Ok: { resultValue: false } },
  { Error: { errorValue: { val: 6 } } },
  { Error: { errorValue: { val: 29 } } },
] as const satisfies FSharpResult_TError<System.Boolean, MyRecord>[];
const resultTTError = [
  { Ok: { resultValue: { val: -21 } } },
  { Error: { errorValue: { val: 6 } } },
  { Error: { errorValue: { val: 29 } } },
] as const satisfies FSharpResult_TTError<MyRecord, MyRecord>[];

// ExternalTag_NamedFields_UnwrapFieldlessTags
