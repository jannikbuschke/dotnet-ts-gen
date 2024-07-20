module TheasoftTests.UsingPaginatedQuery

open Absents.Queries

type PaginatedResultWrapper = {
  Result: PaginatedResult<string>
}
