module Absents.AbstractWorkflow

open System

type NodeId =
  | NodeId of Guid
  member this.Val() =
    match this with
    | NodeId id -> id

  static member New(value: Guid) = NodeId value
  static member New() = Guid.NewGuid() |> NodeId.New
  static member CreateNew() = Guid.NewGuid() |> NodeId.New
  static member ToRawValue(nodeId: NodeId) = nodeId.Val()
  static member FromRawValue(nodeId) = NodeId nodeId

[<CLIMutable>]
type XYPosition =
  { X: int
    Y: int }
  static member zero = { X = 0; Y = 0 }

type INode =
  abstract member NodeId: NodeId
  abstract member Name: string option
  abstract member Description: string option
  abstract member Position: XYPosition

type WorkflowNodeStatus =
  | Completed
  | Pending
  | Active
  | Canceled

type IWorkflowNode =
  inherit INode
  abstract member Status: WorkflowNodeStatus

type EdgeId =
  | EdgeId of string
  member this.Val =
    match this with
    | EdgeId id -> id

  static member New() =
    EdgeId(System.Guid.NewGuid().ToString())

  static member FromNodeIds(src: NodeId, target: NodeId) =
    EdgeId(
      src.Val().ToString()
      + "_"
      + target.Val().ToString()
    )

type PortId =
  | DefaultPort
  | PortId of Guid

type Port = { Id: PortId; Name: string }

type ActivationStrategy =
  | AnyParentCompleted
  | AllParentsCompleted

type Node =
  { NodeId: NodeId
    Name: string option
    Description: string option
    Position: XYPosition
    Status: WorkflowNodeStatus
   }
  static member New(id: NodeId) =
    { NodeId = id
      Name = None
      Status = WorkflowNodeStatus.Pending
      Description = None
      Position = XYPosition.zero
    }

  interface INode with
    member this.NodeId = this.NodeId
    member this.Name = None
    member this.Description = this.Description
    member this.Position = this.Position

type Edge =
  { Id: EdgeId
    Source: NodeId
    Target: NodeId }
  static member New(src: NodeId, target: NodeId) =
    { Id = EdgeId.New()
      Source = src
      Target = target }

  static member Connect(src: INode, target: INode) = Edge.New(src.NodeId, target.NodeId)

type Graph =
  { Edges: Edge list
    Nodes: Node list
    Triggers: NodeId list }
  member this.GetNodeStatus(nodeId: NodeId) =
    let node =
      this.Nodes
      |> List.find (fun x -> x.NodeId = nodeId)

    node.Status

  member this.GetNodesWithStatus(status: WorkflowNodeStatus) =
    this.Nodes
    |> List.filter (fun x -> x.Status = status)

  member this.GetActiveNodes() =
    this.GetNodesWithStatus(WorkflowNodeStatus.Active)

  member this.GetCompletedNodes() =
    this.GetNodesWithStatus(WorkflowNodeStatus.Completed)

  member this.GetPendingNodes() =
    this.GetNodesWithStatus(WorkflowNodeStatus.Pending)

  member this.GetSingleTrigger() =
    match this.Triggers with
    | [] -> failwith "No triggers"
    | [ head ] -> this.Nodes |> List.find (fun x -> x.NodeId = head)
    | _ -> failwith "Multiple triggers"

  static member New(a: Node list, edges: Edge list, triggerId: NodeId) =
    { Edges = edges |> List.distinct
      Nodes = a |> List.distinct
      Triggers = [ triggerId ] }

  member this.GetSourceNode(edge: Edge) =
    this.Nodes
    |> List.find (fun n -> n.NodeId = edge.Source)

  member this.GetTargetNode(edge: Edge) =
    this.Nodes
    |> List.find (fun n -> n.NodeId = edge.Target)

  member this.IncomingEdges(nodeId: NodeId) =
    this.Edges
    |> List.filter (fun e -> e.Target = nodeId)

  member this.OutgoingDefaultEdges(nodeId: NodeId) =
    this.Edges
    |> List.filter (fun e -> e.Source = nodeId)

  member this.Predecessors(nodeId: NodeId) =
    nodeId
    |> this.IncomingEdges
    |> List.map this.GetSourceNode

  member this.Successors(nodeId: NodeId) =
    nodeId
    |> this.OutgoingDefaultEdges
    |> List.map this.GetTargetNode

let (=>>) a b =
  [ a; b ], [ Edge.New(a.NodeId, b.NodeId) ]

let (==>>) (a: Node list, edges: Edge list) b =
  let last = a |> List.last
  let nodes2, edge2 = last =>> b
  (a @ nodes2, edges @ edge2)

let getSuccessors (nodeId: NodeId) (edges: Edge list) (nodes: #INode list) =
  edges
  |> List.filter (fun x -> x.Source = nodeId)
  |> List.map (fun x -> x.Target)
  |> List.map (fun x -> nodes |> List.find (fun y -> y.NodeId = x))

let getTargetNodes (node: #INode) (edges: Edge list) (nodes: #INode list) =
  edges
  |> List.filter (fun x -> x.Source = node.NodeId)
  |> List.map (fun x -> x.Target)
  |> List.map (fun x -> nodes |> List.find (fun y -> y.NodeId = x))

let getAllActiveNodes (nodes: #IWorkflowNode list) =
  nodes
  |> List.filter (fun x -> x.Status = WorkflowNodeStatus.Active)

let getAllPendingNodes (nodes: #IWorkflowNode list) =
  nodes
  |> List.filter (fun x -> x.Status = WorkflowNodeStatus.Pending)

let getAllCompletedNodes (nodes: #IWorkflowNode list) =
  nodes
  |> List.filter (fun x -> x.Status = WorkflowNodeStatus.Completed)

[<RequireQualifiedAccess>]
type NodeEventDto =
  | Activated of NodeId
  | Completed of NodeId

[<RequireQualifiedAccess>]
type NodeEvent =
  | Activated
  | Canceled
  | Completed

[<RequireQualifiedAccess>]
type WorkflowEventDto =
  | Created of Graph: Graph
  | Completed
  | Terminated

[<RequireQualifiedAccess>]
type WorkflowEvent =
  | Created of Graph: Graph
  | Triggered of NodeId
  | Completed
  | Canceled
  | Terminated of Error: string

[<RequireQualifiedAccess>]
type WorkflowEngineEventDto =
  | WorkflowEvent of WorkflowEvent
  | NodeEvent of NodeEvent * NodeId

[<RequireQualifiedAccess>]
type WorkflowEngineEvent =
  | WorkflowEvent of WorkflowEvent
  | NodeEvent of NodeEvent * NodeId
  static member FromDto(dto: WorkflowEngineEventDto) =
    match dto with
    | WorkflowEngineEventDto.WorkflowEvent e -> WorkflowEngineEvent.WorkflowEvent e
    | WorkflowEngineEventDto.NodeEvent (nodeEvent, nodeId) -> WorkflowEngineEvent.NodeEvent(nodeEvent, nodeId)

  static member ToDto(e: WorkflowEngineEvent) =
    match e with
    | WorkflowEngineEvent.WorkflowEvent e -> WorkflowEngineEventDto.WorkflowEvent e
    | WorkflowEngineEvent.NodeEvent (nodeEvent, nodeId) -> WorkflowEngineEventDto.NodeEvent(nodeEvent, nodeId)

  static member Converter = WorkflowEngineEvent.ToDto, WorkflowEngineEvent.FromDto

[<RequireQualifiedAccess>]
type WorkflowCommand =
  | Create of Graph: Graph
  | Trigger of NodeId
  | Terminate of Error: string
  | ActivateNodes

[<RequireQualifiedAccess>]
type NodeCommand =
  | Activate of NodeId
  | Complete of NodeId

type WorkflowSideEffect = | ActivateNodes

[<RequireQualifiedAccess>]
type WorkflowEngineCommand =
  | WorkflowCommand of WorkflowCommand
  | NodeCommand of NodeCommand

type WorkflowState =
  { Graph: Graph }
  member this.GetActiveNodes() = this.Graph.GetActiveNodes()

  static member zero: WorkflowState =
    { Graph =
        { Edges = []
          Nodes = []
          Triggers = [] } }

type EvolveWorkflow = WorkflowState -> WorkflowEngineEvent -> WorkflowState

let toStatus =
  function
  | NodeEvent.Activated -> WorkflowNodeStatus.Active
  | NodeEvent.Canceled -> WorkflowNodeStatus.Canceled
  | NodeEvent.Completed -> WorkflowNodeStatus.Completed

let workflowEvolve (state: WorkflowState) (e: WorkflowEngineEvent) =
  match e with
  | WorkflowEngineEvent.WorkflowEvent workflowEvent ->
    match workflowEvent with
    | WorkflowEvent.Created graph -> { Graph = graph }
    | WorkflowEvent.Triggered nodeId ->
      let nodes =
        state.Graph.Nodes
        |> List.map (fun x ->
          if x.NodeId = nodeId then
            { x with Status = toStatus NodeEvent.Completed }
          else
            x)
      let graph = { state.Graph with Nodes = nodes }
      { WorkflowState.Graph = graph }
    | WorkflowEvent.Completed -> failwith "todo"
    | WorkflowEvent.Canceled -> failwith "todo"
    | WorkflowEvent.Terminated error -> failwith "todo"
  | WorkflowEngineEvent.NodeEvent (nodeEvent, nodeId) ->
    let nodes =
      state.Graph.Nodes
      |> List.map (fun x ->
        if x.NodeId = nodeId then
          { x with Status = toStatus nodeEvent }
        else
          x)
    let graph = { state.Graph with Nodes = nodes }
    { Graph = graph }

let init (e: WorkflowEngineEvent) : WorkflowState =
  match e with
  | WorkflowEngineEvent.WorkflowEvent workflowEvent ->
    match workflowEvent with
    | WorkflowEvent.Created graph -> { WorkflowState.Graph = graph }
    | _ -> failwith "not an initial event"
  | WorkflowEngineEvent.NodeEvent (nodeEvent, id) -> failwith "not an initial event"

let decideWorkflowCommand (state: WorkflowState) (command: WorkflowCommand) =
  match command with
  | WorkflowCommand.Create graph ->
    if graph.Nodes.IsEmpty then
      Result.Error "Cannot create graph without nodes"
    else
      Result.Ok [ WorkflowEngineEvent.WorkflowEvent(WorkflowEvent.Created graph) ]
  | WorkflowCommand.Trigger nodeId ->
    let trigger =
      state.Graph.Triggers
      |> List.tryFind (fun n -> n = nodeId)

    match trigger with
    | Some triggerId ->
      let triggerCompleted = WorkflowEngineEvent.NodeEvent(NodeEvent.Completed, triggerId)
      let workflowTriggered =
        WorkflowEngineEvent.WorkflowEvent(WorkflowEvent.Triggered triggerId)
      let events = [ workflowTriggered; triggerCompleted ]
      Result.Ok events
    | None -> Result.Error $"Trigger {nodeId.Val().ToString()} not found"
  | WorkflowCommand.Terminate error -> Result.Ok [ WorkflowEngineEvent.WorkflowEvent(WorkflowEvent.Terminated error) ]
  | WorkflowCommand.ActivateNodes ->
    let completedNodes = state.Graph.GetCompletedNodes()

    let successors =
      completedNodes
      |> List.collect (fun x -> state.Graph.Successors(x.NodeId))

    let pendingNodes =
      successors
      |> List.filter (fun x -> x.Status = WorkflowNodeStatus.Pending)

    pendingNodes
    |> List.choose (fun node ->
      WorkflowEngineEvent.NodeEvent(NodeEvent.Activated, node.NodeId)
      |> Some)
    |> Result.Ok

let decideNodeCommand (workflow: WorkflowState) (command: NodeCommand) =
  match command with
  | NodeCommand.Complete nodeId -> Result.Ok [ WorkflowEngineEvent.NodeEvent(NodeEvent.Completed, nodeId) ]
  | NodeCommand.Activate nodeId -> Result.Ok [ WorkflowEngineEvent.NodeEvent(NodeEvent.Activated, nodeId) ]

let decide (workflow: WorkflowState) (command: WorkflowEngineCommand) =
  match command with
  | WorkflowEngineCommand.WorkflowCommand workflowCommand -> decideWorkflowCommand workflow workflowCommand
  | WorkflowEngineCommand.NodeCommand nodeCommand -> decideNodeCommand workflow nodeCommand

let getActivatedNodes events =
  events
  |> List.choose (function
    | WorkflowEngineEvent.WorkflowEvent workflowEvent -> None
    | WorkflowEngineEvent.NodeEvent (nodeEvent, nodeId) ->
      match nodeEvent with
      | NodeEvent.Activated -> Some nodeId
      | _ -> None)

