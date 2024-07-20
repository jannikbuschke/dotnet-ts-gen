module Test.TheasoftTests

open System.Linq
open Absents
open Absents.AbsentApprovalApi
open Absents.Queries
open TsGen
open Xunit
open TsGen.Collect

let deps<'t>() =
  let init = TsGen.Collect.init PredefinedTypes.defaultTypes
  let deps = init.getDependencies typedefof<'t>
  let imports = deps |> Seq.map Signature.getModuleName |> Seq.toList
  imports

let expectDeps<'t>(expected: string list) =
  let imports = deps<'t>()
  expected |> List.iter(fun expected ->
    Assert.Contains(expected, imports))  
  ()

[<Fact>]
let ``RequestAbsentApproval should give correct dependencies``() =
  expectDeps<RequestAbsentApproval> ["Microsoft_FSharp_Core"; "System"; "Absents"]
  // let imports = deps<RequestAbsentApproval>()
  // ["Microsoft_FSharp_Core"; "System"; "Absents"] |> List.iter(fun expected ->
  //   Assert.Contains(expected, imports))  
  ()

let expectModuleName(t:System.Type,expected: string) =
  let moduleName = Signature.getModuleName t
  Expect.eq moduleName expected
  ()

[<Theory>]
[<InlineData(typeof<Absents.RequestAbsentApproval>, "Absents")>]
[<InlineData(typeof<Absents.AbsentApprovalApi.AbsentApprovalRequest>, "Absents_AbsentApprovalApi")>]
[<InlineData(typeof<Absents.AbsentApprovalApi.X>, "Absents_AbsentApprovalApi")>]
[<InlineData(typeof<Absents.AbstractWorkflow.Node>, "Absents_AbstractWorkflow")>]
[<InlineData(typeof<PaginatedResult<string>>, "Absents_Queries")>]
let ``Should give correct module names``(t,expectedName:string)=
  expectModuleName(t, expectedName)

[<Fact>]
let ``Should give correct dependencies``() =
  expectDeps<AbsentApprovalApi.AbsentApprovalRequest> ["Absents"]
  expectDeps<AbsentApprovalApi.X> ["System"]
  expectDeps<Absents.AbstractWorkflow.Node> ["Microsoft_FSharp_Core"; "System"; "Absents_AbstractWorkflow"]
  ()

[<Fact>]
let ``Should render AbsenceRequestEvent`` () =
  let typedef0 = renderTypeDef typedefof<AbsenceRequestEvent>
  Expect.similar
    typedef0
    """export type AbsenceRequestEvent_Case_ApprovalWorkflowCreated = { Case: "ApprovalWorkflowCreated", Fields: AbsentApprovalWorkflowCreated }
export type AbsenceRequestEvent_Case_ApprovalNodeAdded = { Case: "ApprovalNodeAdded", Fields: { item1: AbsentApprovalNode, item2: Microsoft_FSharp_Collections.FSharpList<Absents_AbstractWorkflow.Edge> } }
export type AbsenceRequestEvent_Case_ApprovalAdded = { Case: "ApprovalAdded", Fields: { item1: Approval, item2: Absents_AbstractWorkflow.NodeId } }
export type AbsenceRequestEvent_Case_ApprovalRemoved = { Case: "ApprovalRemoved", Fields: { item1: UserId, item2: Absents_AbstractWorkflow.NodeId } }
export type AbsenceRequestEvent_Case_ApprovalStepReviewed = { Case: "ApprovalStepReviewed", Fields: { item1: Absents_AbstractWorkflow.NodeId, item2: Reviewed } }
export type AbsenceRequestEvent_Case_ApprovalStepActivated = { Case: "ApprovalStepActivated", Fields: Absents_AbstractWorkflow.NodeId }
export type AbsenceRequestEvent_Case_ApprovalStepCanceled = { Case: "ApprovalStepCanceled", Fields: Absents_AbstractWorkflow.NodeId }
export type AbsenceRequestEvent_Case_FinallyReviewed = { Case: "FinallyReviewed", Fields: Reviewed }
export type AbsenceRequestEvent_Case_FinalDecisionChanged = { Case: "FinalDecisionChanged", Fields: Reviewed }
export type AbsenceRequestEvent_Case_PersonUserResolved = { Case: "PersonUserResolved", Fields: { item1: PersonId, item2: UserId } }
export type AbsenceRequestEvent_Case_WorkflowEvent = { Case: "WorkflowEvent", Fields: Absents_AbstractWorkflow.WorkflowEngineEvent }
export type AbsenceRequestEvent_Case_Canceled = { Case: "Canceled" }
export type AbsenceRequestEvent_Case_Deleted = { Case: "Deleted" }
export type AbsenceRequestEvent = AbsenceRequestEvent_Case_ApprovalWorkflowCreated | AbsenceRequestEvent_Case_ApprovalNodeAdded | AbsenceRequestEvent_Case_ApprovalAdded | AbsenceRequestEvent_Case_ApprovalRemoved | AbsenceRequestEvent_Case_ApprovalStepReviewed | AbsenceRequestEvent_Case_ApprovalStepActivated | AbsenceRequestEvent_Case_ApprovalStepCanceled | AbsenceRequestEvent_Case_FinallyReviewed | AbsenceRequestEvent_Case_FinalDecisionChanged | AbsenceRequestEvent_Case_PersonUserResolved | AbsenceRequestEvent_Case_WorkflowEvent | AbsenceRequestEvent_Case_Canceled | AbsenceRequestEvent_Case_Deleted
export type AbsenceRequestEvent_Case = "ApprovalWorkflowCreated" | "ApprovalNodeAdded" | "ApprovalAdded" | "ApprovalRemoved" | "ApprovalStepReviewed" | "ApprovalStepActivated" | "ApprovalStepCanceled" | "FinallyReviewed" | "FinalDecisionChanged" | "PersonUserResolved" | "WorkflowEvent" | "Canceled" | "Deleted"
"""
