//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

export type Unit = null;
export type FSharpResult_Case_Ok<T, TError> = { Ok: { resultValue: T } };
export type FSharpResult_Case_Error<T, TError> = {
  Error: { errorValue: TError };
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
// GENERIC RECORD DU: T
export type FSharpResult_Case_Ok_T<T, TError> = { Ok: T };
export type FSharpResult_Case_Error_T<T, TError> = {
  Error: { errorValue: TError };
};
export type FSharpResult_T<T, TError> =
  | FSharpResult_Case_Ok_T<T, TError>
  | FSharpResult_Case_Error_T<T, TError>;
export type FSharpResult_T_Case = 'Ok' | 'Error';
export const FSharpResult_T_AllCases = [
  'Ok',
  'Error',
] satisfies FSharpResult_T_Case[];
export function isFSharpResult_T_Case(
  value: any
): value is FSharpResult_T_Case {
  return FSharpResult_T_AllCases.includes(value);
}
// GENERIC RECORD DU: TError
export type FSharpResult_Case_Ok_TError<T, TError> = { Ok: { resultValue: T } };
export type FSharpResult_Case_Error_TError<T, TError> = { Error: TError };
export type FSharpResult_TError<T, TError> =
  | FSharpResult_Case_Ok_TError<T, TError>
  | FSharpResult_Case_Error_TError<T, TError>;
export type FSharpResult_TError_Case = 'Ok' | 'Error';
export const FSharpResult_TError_AllCases = [
  'Ok',
  'Error',
] satisfies FSharpResult_TError_Case[];
export function isFSharpResult_TError_Case(
  value: any
): value is FSharpResult_TError_Case {
  return FSharpResult_TError_AllCases.includes(value);
}
// GENERIC RECORD DU: T,TError
export type FSharpResult_Case_Ok_TTError<T, TError> = { Ok: T };
export type FSharpResult_Case_Error_TTError<T, TError> = { Error: TError };
export type FSharpResult_TTError<T, TError> =
  | FSharpResult_Case_Ok_TTError<T, TError>
  | FSharpResult_Case_Error_TTError<T, TError>;
export type FSharpResult_TTError_Case = 'Ok' | 'Error';
export const FSharpResult_TTError_AllCases = [
  'Ok',
  'Error',
] satisfies FSharpResult_TTError_Case[];
export function isFSharpResult_TTError_Case(
  value: any
): value is FSharpResult_TTError_Case {
  return FSharpResult_TTError_AllCases.includes(value);
}

export type FSharpOption<T> = T | null;
export type FSharpOption_T<T> = T | null;
