namespace Absents

open System
open System.Text.Json.Serialization
open Absents.AbstractWorkflow
open TheasoftTests.Ids

[<RequireQualifiedAccess>]
type ApprovalResult =
  | Pending
  | Approved
  | Rejected

[<RequireQualifiedAccess>]
type WorkflowResult =
  | Pending
  | Approved
  | Rejected
  | Canceled
  static member FromApprovalResult(approvalResult: ApprovalResult) =
    match approvalResult with
    | ApprovalResult.Pending -> WorkflowResult.Pending
    | ApprovalResult.Approved -> WorkflowResult.Approved
    | ApprovalResult.Rejected -> WorkflowResult.Rejected

type ApprovalAddedReason =
  | StaticalyDefined
  | StaticalyDefinedByGroup of GroupId: UserId
  | AdminFallbackNoPersonsWithUsers
  | ManuallyAdded of AddedBy: UserId

[<CLIMutable>]
type Approval =
  { UserId: UserId
    Reason: ApprovalAddedReason
    ReviewedAt: DateTimeOffset option
    mutable Result: ApprovalResult
    mutable Comment: string option }
  static member New(userId: UserId, reason: ApprovalAddedReason) =
    { UserId = userId
      Reason = reason
      ReviewedAt = None
      Result = ApprovalResult.Pending
      Comment = None }

[<CLIMutable>]
type ApprovalStep =
  { Approvals: Approval list
    NoteForApprovers: string option
    ApproversWithoutUser: (PersonId * ApprovalAddedReason) list
    CanChangeFinalDecision: Skippable<bool>
    RejectionIsFinal: Skippable<bool>
    mutable Result: ApprovalResult }

[<RequireQualifiedAccess>]
type AbsentApprovalNodeDetails =
  | Approval of ApprovalStep
  | Trigger
type AbsentApprovalNode =
  { NodeId: NodeId
    Name: string option
    Details: AbsentApprovalNodeDetails
    UpdatedAt: DateTimeOffset
    BasedOnConfigNode: NodeId option }

type AbsentApprovalGraph =
  { Nodes: AbsentApprovalNode list
    BasedOnConfig: string option
    BasedOnConfigId: int option }

[<CLIMutable>]
type Period =
  { StartDate: DateOnly
    StartTime: TimeOnly option
    EndDate: DateOnly
    EndTime: TimeOnly option }
  static member New(startDate: DateOnly, endDate: DateOnly) =
    { StartDate = startDate
      StartTime = None
      EndDate = endDate
      EndTime = None }

  static member New(date: DateOnly) =
    { StartDate = date
      StartTime = None
      EndDate = date
      EndTime = None }

  static member New(startDate: DateOnly, endDate: DateOnly, endTime: TimeOnly) =
    { StartDate = startDate
      StartTime = None
      EndDate = endDate
      EndTime = Some endTime }

  static member New(startDate: DateOnly, startTime: TimeOnly, endDate: DateOnly) =
    { StartDate = startDate
      StartTime = Some startTime
      EndDate = endDate
      EndTime = None }

  static member New(startDate: DateOnly, startTime: TimeOnly, endDate: DateOnly, endTime: TimeOnly) =
    { StartDate = startDate
      StartTime = Some startTime
      EndDate = endDate
      EndTime = Some endTime }

[<CLIMutable>]
type AbsentApprovalListItem =
  { 
    Result: ApprovalResult
    PersonStatusId: int
}

[<RequireQualifiedAccess>]
type Recipient =
  | UserId of UserId
  | PersonId of PersonId
  | Email of string

type Email =
  { To: Recipient list
    Subject: string
    Body: string }
  static member New(recipient: PersonId, subject: string, body: string) =
    { To = [ Recipient.PersonId recipient ]
      Subject = subject
      Body = body }

  static member New(recipient: UserId, subject: string, body: string) =
    { To = [ Recipient.UserId recipient ]
      Subject = subject
      Body = body }

type SideEffect =
  | SendEmail of Email
  | RunAbsenceApprovalWorkflow of int

type ApprovalDecision =
  | Approve
  | Reject

[<CLIMutable>]
type Review =
  { Decision: ApprovalDecision
    Comment: string option }

type Reviewed =
  { Result: ApprovalResult
    Comment: string option }

[<CLIMutable>]
type CastDto =
  { CastId: int
    PersonId: int
    PersonName: string }

[<CLIMutable>]
type ActivityDto =
  { ActivityId: int
    ProductionTitle: string
    Date: DateOnly
    Start: DateTime
    End: DateTime
    ActivityTypeName: string
    LocationName: string
    Casts: CastDto seq }

type Permission =
  { ActiveNodesWhereUserIsApprover: AbsentApprovalNode list
    IsAdmin: bool
    CanReview: bool
    CanChangeFinalDecision: bool
    CanSetFinal: bool }
