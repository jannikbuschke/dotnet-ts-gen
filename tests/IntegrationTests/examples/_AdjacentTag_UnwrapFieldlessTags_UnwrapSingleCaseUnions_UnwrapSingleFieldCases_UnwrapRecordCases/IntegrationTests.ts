//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as ___ from './___';
import * as Microsoft_FSharp_Core from './Microsoft_FSharp_Core';
import * as System from './System';

export type MultiCaseMultiFields_Case_NoField = 'NoField';
export type MultiCaseMultiFields_Case_OneAnonField = {
  Case: 'OneAnonField';
  Fields: ___.f__AnonymousType113051835<System.Int32, System.String>;
};
export type MultiCaseMultiFields_Case_OneField = {
  Case: 'OneField';
  Fields: System.Int32;
};
export type MultiCaseMultiFields_Case_TwoFields = {
  Case: 'TwoFields';
  Fields: { item1: System.String; item2: System.Boolean };
};
export type MultiCaseMultiFields_Case_RecordField = {
  Case: 'RecordField';
  Fields: Record;
};
export type MultiCaseMultiFields_Case_Records = {
  Case: 'Records';
  Fields: { item1: Record; item2: Record; item3: System.Boolean };
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

export type Record = {
  x: System.String;
};
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
export type GenericDu_Case_OneField<a, b> = { Case: 'OneField'; Fields: a };
export type GenericDu_Case_OneAnonFieldBool<a, b> = {
  Case: 'OneAnonFieldBool';
  Fields: ___.f__AnonymousType174717633<System.Boolean>;
};
export type GenericDu_Case_OneAnonFieldAOption<a, b> = {
  Case: 'OneAnonFieldAOption';
  Fields: ___.f__AnonymousType174717633<Microsoft_FSharp_Core.FSharpOption<a>>;
};
export type GenericDu_Case_TwoFields<a, b> = {
  Case: 'TwoFields';
  Fields: { item1: b; item2: System.Int32 };
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
// GENERIC RECORD DU: a
export type GenericDu_Case_NoField_a<a, b> = 'NoField';
export type GenericDu_Case_OneField_a<a, b> = { Case: 'OneField'; Fields: a };
export type GenericDu_Case_OneAnonFieldBool_a<a, b> = {
  Case: 'OneAnonFieldBool';
  Fields: ___.f__AnonymousType174717633<System.Boolean>;
};
export type GenericDu_Case_OneAnonFieldAOption_a<a, b> = {
  Case: 'OneAnonFieldAOption';
  Fields: ___.f__AnonymousType174717633<Microsoft_FSharp_Core.FSharpOption<a>>;
};
export type GenericDu_Case_TwoFields_a<a, b> = {
  Case: 'TwoFields';
  Fields: { item1: b; item2: System.Int32 };
};
export type GenericDu_a<a, b> =
  | GenericDu_Case_NoField_a<a, b>
  | GenericDu_Case_OneField_a<a, b>
  | GenericDu_Case_OneAnonFieldBool_a<a, b>
  | GenericDu_Case_OneAnonFieldAOption_a<a, b>
  | GenericDu_Case_TwoFields_a<a, b>;
export type GenericDu_a_Case =
  | 'NoField'
  | 'OneField'
  | 'OneAnonFieldBool'
  | 'OneAnonFieldAOption'
  | 'TwoFields';
export const GenericDu_a_AllCases = [
  'NoField',
  'OneField',
  'OneAnonFieldBool',
  'OneAnonFieldAOption',
  'TwoFields',
] satisfies GenericDu_a_Case[];
export function isGenericDu_a_Case(value: any): value is GenericDu_a_Case {
  return GenericDu_a_AllCases.includes(value);
}
// GENERIC RECORD DU: b
export type GenericDu_Case_NoField_b<a, b> = 'NoField';
export type GenericDu_Case_OneField_b<a, b> = { Case: 'OneField'; Fields: a };
export type GenericDu_Case_OneAnonFieldBool_b<a, b> = {
  Case: 'OneAnonFieldBool';
  Fields: ___.f__AnonymousType174717633<System.Boolean>;
};
export type GenericDu_Case_OneAnonFieldAOption_b<a, b> = {
  Case: 'OneAnonFieldAOption';
  Fields: ___.f__AnonymousType174717633<Microsoft_FSharp_Core.FSharpOption<a>>;
};
export type GenericDu_Case_TwoFields_b<a, b> = {
  Case: 'TwoFields';
  Fields: { item1: b; item2: System.Int32 };
};
export type GenericDu_b<a, b> =
  | GenericDu_Case_NoField_b<a, b>
  | GenericDu_Case_OneField_b<a, b>
  | GenericDu_Case_OneAnonFieldBool_b<a, b>
  | GenericDu_Case_OneAnonFieldAOption_b<a, b>
  | GenericDu_Case_TwoFields_b<a, b>;
export type GenericDu_b_Case =
  | 'NoField'
  | 'OneField'
  | 'OneAnonFieldBool'
  | 'OneAnonFieldAOption'
  | 'TwoFields';
export const GenericDu_b_AllCases = [
  'NoField',
  'OneField',
  'OneAnonFieldBool',
  'OneAnonFieldAOption',
  'TwoFields',
] satisfies GenericDu_b_Case[];
export function isGenericDu_b_Case(value: any): value is GenericDu_b_Case {
  return GenericDu_b_AllCases.includes(value);
}
// GENERIC RECORD DU: a,b
export type GenericDu_Case_NoField_ab<a, b> = 'NoField';
export type GenericDu_Case_OneField_ab<a, b> = { Case: 'OneField'; Fields: a };
export type GenericDu_Case_OneAnonFieldBool_ab<a, b> = {
  Case: 'OneAnonFieldBool';
  Fields: ___.f__AnonymousType174717633<System.Boolean>;
};
export type GenericDu_Case_OneAnonFieldAOption_ab<a, b> = {
  Case: 'OneAnonFieldAOption';
  Fields: ___.f__AnonymousType174717633<Microsoft_FSharp_Core.FSharpOption<a>>;
};
export type GenericDu_Case_TwoFields_ab<a, b> = {
  Case: 'TwoFields';
  Fields: { item1: b; item2: System.Int32 };
};
export type GenericDu_ab<a, b> =
  | GenericDu_Case_NoField_ab<a, b>
  | GenericDu_Case_OneField_ab<a, b>
  | GenericDu_Case_OneAnonFieldBool_ab<a, b>
  | GenericDu_Case_OneAnonFieldAOption_ab<a, b>
  | GenericDu_Case_TwoFields_ab<a, b>;
export type GenericDu_ab_Case =
  | 'NoField'
  | 'OneField'
  | 'OneAnonFieldBool'
  | 'OneAnonFieldAOption'
  | 'TwoFields';
export const GenericDu_ab_AllCases = [
  'NoField',
  'OneField',
  'OneAnonFieldBool',
  'OneAnonFieldAOption',
  'TwoFields',
] satisfies GenericDu_ab_Case[];
export function isGenericDu_ab_Case(value: any): value is GenericDu_ab_Case {
  return GenericDu_ab_AllCases.includes(value);
}

export type GenericDu0_Case_OneField<a> = a;
export type GenericDu0<a> = GenericDu0_Case_OneField<a>;
export type GenericDu0_Case = 'OneField';
export const GenericDu0_AllCases = ['OneField'] satisfies GenericDu0_Case[];
export function isGenericDu0_Case(value: any): value is GenericDu0_Case {
  return GenericDu0_AllCases.includes(value);
}
// GENERIC RECORD DU: a
export type GenericDu0_Case_OneField_a<a> = a;
export type GenericDu0_a<a> = GenericDu0_Case_OneField_a<a>;
export type GenericDu0_a_Case = 'OneField';
export const GenericDu0_a_AllCases = ['OneField'] satisfies GenericDu0_a_Case[];
export function isGenericDu0_a_Case(value: any): value is GenericDu0_a_Case {
  return GenericDu0_a_AllCases.includes(value);
}

export type MyRecord = {
  val: System.Int32;
};
