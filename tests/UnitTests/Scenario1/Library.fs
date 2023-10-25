module Test.TheasoftTests

open System.Linq
open Absents
open Absents.Queries
open TheasoftTests.Ids
open TinyTypeGen
open Expecto

let expectModuleName (t: System.Type, expected: string) =
  let moduleName = Signature.getModuleName t
  Expect.eq moduleName expected
  ()

let tests =
  testList
    "Scenario1"
    [
      ptestCase
        "Should render AbsenceRequestEvent0"
        (fun () ->
          typedefof<AbsenceRequestEvent>
          |> renderCustomTypeDef Config.defaultJsonUnionEncoding
          |> Expect.stringStart
            """
export type AbsenceRequestEvent_Case_ApprovalStepActivated = { Case: "ApprovalStepActivated", Fields: Absents_AbstractWorkflow.NodeId }
export type AbsenceRequestEvent_Case_ApprovalStepCanceled = { Case: "ApprovalStepCanceled", Fields: Absents_AbstractWorkflow.NodeId }
}
"""
        )
    ]
