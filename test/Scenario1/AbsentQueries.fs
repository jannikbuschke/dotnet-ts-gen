module Absents.Queries

open System
open System.Text.Json.Serialization
open Absents.AbstractWorkflow
open Giraffe
open Microsoft.AspNetCore.Http
open System.Linq
open Microsoft.Extensions.Logging
open Microsoft.FSharp.Linq.RuntimeHelpers
open TheasoftTests.Ids
open TinyTypeGen.Giraffe

[<RequireQualifiedAccess>]
type ActivityType =
  | StartOfAbsence
  | EndOfAbsence
  | Premiere
  | Performance
  | Secret
  | PossibleCast

[<RequireQualifiedAccess>]
type CastType =
  | Secret
  | PossibleCast

type OverlapStatus =
  | NotOverlapping
  | Overlapping
  | SameAsReturnDay

type OffsetReason =
  | Mask
  | Other

type ActivityInPeriod =
  { ActivityId: int option
    Name: string
    Date: DateOnly
    Time: TimeOnly option
    ActivityTypeName: string
    ActivityType: ActivityType
    OverlapStatus: OverlapStatus }

type ActivitiesAndBasicCastsInPeriod =
  { StartOfAbsence: ActivityInPeriod
    EndOfAbsence: ActivityInPeriod
    GroupedActivities: (OverlapStatus * ActivityInPeriod list) list }

[<CLIMutable>]
type GetActivitiesAndCasts = { Period: Period }

[<CLIMutable>]
type GetMyRequests = { Page: int; PageSize: int }

type MyRequestListItem =
  { Result: WorkflowResult option
    CreatedAt: DateOnly option
    Period: Period }

[<RequireQualifiedAccess>]
type StatusFilter =
  | All
  | Pending
  | Approved
  | Rejected

[<RequireQualifiedAccess>]
type MyDecisionFilter =
  | All
  | Pending
  | PendingAndActive
  | Approved
  | Rejected

type Sort =
  | RequestedAtDesc
  | RequestedAtAsc

[<CLIMutable>]
type GetMyApprovalsRequest =
  { StatusFilter: StatusFilter
    MyDecision: MyDecisionFilter
    PersonId: PersonId option
    Page: int
    Sort: Sort }

type PaginatedResult<'T> = { Items: 'T list; Count: int }

type SortOrder =
  | Asc
  | Desc

[<CLIMutable>]
type MyApprovalListItem =
  { Result: WorkflowResult option
    CreatedAt: DateOnly option
    Period: Period }

[<CLIMutable>]
type GetApprovalWorkflow = { Id: int }

type AbsentApprovalForReview =
  { ShiftTypeName: string
    ActivitiesAndBasicCastsInPeriod: ActivitiesAndBasicCastsInPeriod
    CurrentNode: AbsentApprovalNode option
    CurrentNodeReason: ApprovalAddedReason option
    Workflow: AbsenceRequestApproval
    CanReview: bool
    RejectionWouldBeFinal: bool
    IsFinalNode: bool
    Events: AbsenceEventEnvelope list
    CanChangeFinalDecision: bool
    CanSetFinal: bool
    ActiveNodes: AbsentApprovalNode list
    CompletedNodes: AbsentApprovalNode list
    PendingNodes: AbsentApprovalNode list
    CanResolvePersonUsers: bool
    Status: WorkflowNodeStatus }
