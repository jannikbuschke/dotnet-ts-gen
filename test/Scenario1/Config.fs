
namespace Absents

open System.Text.Json.Serialization
open Absents.AbstractWorkflow

type StaticApprovalStep =
  { NoteForApprovers: string option
    RejectionIsFinal: Skippable<bool>
    CanChangeFinalDecision: Skippable<bool> }

type ProductionRoleApprovalStep =
  { NoteForApprovers: string option
    RejectionIsFinal: Skippable<bool>
    CanChangeFinalDecision: Skippable<bool> }

type ContractTypeCategoryIdOrContractTypeId =
  | ContractTypeCategoryId of int16
  | ContractTypeId of int

type ContractTypeOrContractTypeCategoryCondition =
  { Values: ContractTypeCategoryIdOrContractTypeId list }

type AbsentApprovalConfigNodeDetails =
  | Trigger
  | ContractTypeOrContractTypeCategoryCondition of ContractTypeOrContractTypeCategoryCondition
  | StaticApproval of StaticApprovalStep
  | ProductionRoleApproval of ProductionRoleApprovalStep

type GraphNode<'details> =
  { NodeId: NodeId
    Name: string option
    Description: string option
    Details: 'details
    Position: XYPosition }
  interface INode with
    member this.NodeId = this.NodeId
    member this.Position = this.Position

    member this.Description = this.Description
    member this.Name = this.Name

  static member New(nodeId,details: 'details) =
    { NodeId = nodeId
      Name = None
      Description = None
      Details = details
      Position = XYPosition.zero }

type AbsentApprovalConfigNode = GraphNode<AbsentApprovalConfigNodeDetails>

[<CLIMutable>]
type AbsentApprovalConfig =
  { Name: string
    Nodes: AbsentApprovalConfigNode list
    Edges: Edge list
  }
