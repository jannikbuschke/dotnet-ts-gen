namespace IntegrationTests

open FsApi

type EnumLikeUnion =
  | A
  | B
  | C

type SingleCaseMultiField = | Value of int * bool * {| Age: int |}

type SingleCaseUnion = | Value of int

type MyRecord = { Val: int }

type MultiCaseMultiFields =
  | NoField
  | OneAnonField of
    {|
      Name: string
      Age: int
    |}
  | OneField of int
  | TwoFields of string * bool
  | RecordField of Record
  | Records of Record * Record * bool

type GenericDu0<'a> = | OneField of 'a

type GenericDu<'a, 'b> =
  | NoField
  | OneField of 'a
  | OneAnonFieldBool of {| Value: bool |}
  | OneAnonFieldAOption of {| Value: 'a option |}
  | TwoFields of 'b * int
