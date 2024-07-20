namespace Absents

open System
open Absents.AbstractWorkflow
open Microsoft.FSharp.Core
open System.Linq
open TheasoftTests.Ids

type AbsenceRequestApproval =
  { AbsentApprovalWorkflowId: int
    Comment: string option
    CreatedByUserId: UserId
    CreatedByPersonId: PersonId
    CreatedAt: DateTimeOffset
    Period: Period
    Result: WorkflowResult
    Nodes: AbsentApprovalNode list
    Edges: Edge list
    BasedOnConfigs: int list
    ApprovalWorkflow: WorkflowState }

type RequestAbsentApproval =
  { Start: DateOnly
    End: DateOnly
    StartTime: TimeOnly option
    EndTime: TimeOnly option
    Result: ApprovalResult
    Comment: string option }

type AbsentApprovalWorkflowCreated =
  {
    AbsentApprovalWorkflowId: int
    CreatedByUserId: UserId
    CreatedByPersonId: PersonId
    Period: Period
    Nodes: AbsentApprovalNode list
    Edges: Edge list
    Comment: string option
    BasedOnConfigs: int list }

type AbsentApprovalCommand =
  | RequestAbsentApproval of RequestAbsentApproval
  | ReviewApprovalStep of nodeId: NodeId * review: Review
  | SetFinalDecision of Review
  | ChangeFinalDecision of Review
  | ResolvePersonUser of PersonId * UserId
  | Cancel
  | Delete
  | AddApprover of UserId: UserId * NodeId: NodeId
  | RemoveApprover of UserId: UserId * NodeId: NodeId
  | AddApprovalStep of Users: UserId list * NoteForApprovers: string option * CanChangeFinalDecision: bool * RejectionIsFinal: bool
  | ApprovalWorkflowCommand of WorkflowEngineCommand

[<RequireQualifiedAccess>]
type PureAbsentApprovalCommand =
  | ReviewApprovalStep of nodeId: NodeId * review: Review * permission: Permission
  | SetFinalDecision of Review * permission: Permission
  | ChangeFinalDecision of Review * permission: Permission
  | ResolvePersonUser of PersonId * UserId
  | Cancel
  | Delete
  | AddApprover of UserId: UserId * NodeId: NodeId
  | RemoveApprover of UserId: UserId * NodeId: NodeId
  | AddApprovalStep of Users: UserId list * NoteForApprovers: string option * CanChangeFinalDecision: bool * RejectionIsFinal: bool
  | ApprovalWorkflowCommand of WorkflowEngineCommand

type ApprovalStepEvent =
  | NodeAdded of AbsentApprovalNode * Edge list
  | ApprovalAdded of Approval
  | ApprovalRemoved of UserId
  | StepReviewed of Reviewed
  | StepActivated
  | StepCanceled

type AbsenceRequestEvent =
  | ApprovalWorkflowCreated of AbsentApprovalWorkflowCreated
  | ApprovalNodeAdded of AbsentApprovalNode * Edge list
  | ApprovalAdded of Approval * NodeId
  | ApprovalRemoved of UserId * NodeId
  | ApprovalStepReviewed of NodeId * Reviewed
  | ApprovalStepActivated of NodeId
  | ApprovalStepCanceled of NodeId
  | FinallyReviewed of Reviewed
  | FinalDecisionChanged of Reviewed
  | PersonUserResolved of PersonId * UserId
  | WorkflowEvent of WorkflowEngineEvent
  | Canceled
  | Deleted

type AbsenceEventEnvelope = {
  Data: AbsenceRequestEvent
}
  
