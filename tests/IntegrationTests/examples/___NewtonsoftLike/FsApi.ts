//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as System from './System';

export type Record = {
  foo: System.Int32;
};
export type Union_Case_CaseA = { Case: 'CaseA'; Fields: [System.Int32] };
export type Union_Case_CaseB = { Case: 'CaseB'; Fields: [System.String] };
export type Union_Case_CaseC = { Case: 'CaseC'; Fields: [Record] };
export type Union = Union_Case_CaseA | Union_Case_CaseB | Union_Case_CaseC;
export type Union_Case = 'CaseA' | 'CaseB' | 'CaseC';
export const Union_AllCases = [
  'CaseA',
  'CaseB',
  'CaseC',
] satisfies Union_Case[];
export function isUnion_Case(value: any): value is Union_Case {
  return Union_AllCases.includes(value);
}
