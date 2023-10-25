//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as System from './System';

export type Record = {
  foo: System.Int32;
};
export type Union_Case_CaseA = ['CaseA', System.Int32];
export type Union_Case_CaseB = ['CaseB', System.String];
export type Union_Case_CaseC = ['CaseC', Record];
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
