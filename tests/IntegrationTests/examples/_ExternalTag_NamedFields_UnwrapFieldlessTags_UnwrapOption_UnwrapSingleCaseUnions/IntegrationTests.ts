//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as ___ from './___';
import * as System from './System';
import * as FsApi from './FsApi';
import * as Microsoft_FSharp_Core from './Microsoft_FSharp_Core';

export type MultiCaseMultiFields_Case_NoField = 'NoField';
export type MultiCaseMultiFields_Case_OneAnonField = {
  OneAnonField: {
    item: ___.f__AnonymousType113051835<System.Int32, System.String>;
  };
};
export type MultiCaseMultiFields_Case_OneField = {
  OneField: { item: System.Int32 };
};
export type MultiCaseMultiFields_Case_TwoFields = {
  TwoFields: { item1: System.String; item2: System.Boolean };
};
export type MultiCaseMultiFields_Case_RecordField = {
  RecordField: { item: FsApi.Record };
};
export type MultiCaseMultiFields_Case_Records = {
  Records: { item1: FsApi.Record; item2: FsApi.Record; item3: System.Boolean };
};
export type MultiCaseMultiFields =
  | MultiCaseMultiFields_Case_NoField
  | MultiCaseMultiFields_Case_OneAnonField
  | MultiCaseMultiFields_Case_OneField
  | MultiCaseMultiFields_Case_TwoFields
  | MultiCaseMultiFields_Case_RecordField
  | MultiCaseMultiFields_Case_Records;
export type MultiCaseMultiFields_Case =
  | 'NoField'
  | 'OneAnonField'
  | 'OneField'
  | 'TwoFields'
  | 'RecordField'
  | 'Records';
export const MultiCaseMultiFields_AllCases = [
  'NoField',
  'OneAnonField',
  'OneField',
  'TwoFields',
  'RecordField',
  'Records',
] satisfies MultiCaseMultiFields_Case[];
export function isMultiCaseMultiFields_Case(
  value: any
): value is MultiCaseMultiFields_Case {
  return MultiCaseMultiFields_AllCases.includes(value);
}

export type SingleCaseUnion_Case_Value = System.Int32;
export type SingleCaseUnion = SingleCaseUnion_Case_Value;
export type SingleCaseUnion_Case = 'Value';
export const SingleCaseUnion_AllCases = [
  'Value',
] satisfies SingleCaseUnion_Case[];
export function isSingleCaseUnion_Case(
  value: any
): value is SingleCaseUnion_Case {
  return SingleCaseUnion_AllCases.includes(value);
}

export type EnumLikeUnion_Case_A = 'A';
export type EnumLikeUnion_Case_B = 'B';
export type EnumLikeUnion_Case_C = 'C';
export type EnumLikeUnion =
  | EnumLikeUnion_Case_A
  | EnumLikeUnion_Case_B
  | EnumLikeUnion_Case_C;
export type EnumLikeUnion_Case = 'A' | 'B' | 'C';
export const EnumLikeUnion_AllCases = [
  'A',
  'B',
  'C',
] satisfies EnumLikeUnion_Case[];
export function isEnumLikeUnion_Case(value: any): value is EnumLikeUnion_Case {
  return EnumLikeUnion_AllCases.includes(value);
}

export type GenericDu_Case_NoField<a, b> = 'NoField';
export type GenericDu_Case_OneField<a, b> = { OneField: { item: a } };
export type GenericDu_Case_OneAnonFieldBool<a, b> = {
  OneAnonFieldBool: { item: ___.f__AnonymousType174717633<System.Boolean> };
};
export type GenericDu_Case_OneAnonFieldAOption<a, b> = {
  OneAnonFieldAOption: {
    item: ___.f__AnonymousType174717633<Microsoft_FSharp_Core.FSharpOption<a>>;
  };
};
export type GenericDu_Case_TwoFields<a, b> = {
  TwoFields: { item1: b; item2: System.Int32 };
};
export type GenericDu<a, b> =
  | GenericDu_Case_NoField<a, b>
  | GenericDu_Case_OneField<a, b>
  | GenericDu_Case_OneAnonFieldBool<a, b>
  | GenericDu_Case_OneAnonFieldAOption<a, b>
  | GenericDu_Case_TwoFields<a, b>;
export type GenericDu_Case =
  | 'NoField'
  | 'OneField'
  | 'OneAnonFieldBool'
  | 'OneAnonFieldAOption'
  | 'TwoFields';
export const GenericDu_AllCases = [
  'NoField',
  'OneField',
  'OneAnonFieldBool',
  'OneAnonFieldAOption',
  'TwoFields',
] satisfies GenericDu_Case[];
export function isGenericDu_Case(value: any): value is GenericDu_Case {
  return GenericDu_AllCases.includes(value);
}

export type GenericDu0_Case_OneField<a> = a;
export type GenericDu0<a> = GenericDu0_Case_OneField<a>;
export type GenericDu0_Case = 'OneField';
export const GenericDu0_AllCases = ['OneField'] satisfies GenericDu0_Case[];
export function isGenericDu0_Case(value: any): value is GenericDu0_Case {
  return GenericDu0_AllCases.includes(value);
}

export type MyRecord = {
  val: System.Int32;
};
