//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

export type Unit = null;
export type FSharpResult_Case_Ok<T, TError> = {
  Case: 'Ok';
  Fields: { resultValue: T };
};
export type FSharpResult_Case_Error<T, TError> = {
  Case: 'Error';
  Fields: { errorValue: TError };
};
export type FSharpResult<T, TError> =
  | FSharpResult_Case_Ok<T, TError>
  | FSharpResult_Case_Error<T, TError>;
export type FSharpResult_Case = 'Ok' | 'Error';
export const FSharpResult_AllCases = [
  'Ok',
  'Error',
] satisfies FSharpResult_Case[];
export function isFSharpResult_Case(value: any): value is FSharpResult_Case {
  return FSharpResult_AllCases.includes(value);
}

export type FSharpOption<T> = T | null;
