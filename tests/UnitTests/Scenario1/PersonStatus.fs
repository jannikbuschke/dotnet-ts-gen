namespace Absents

open System
open System.Collections.Generic
open Absents.AbstractWorkflow

type PersonApproverStatus =
  // uy means unsigned byte
  | Pending = 1uy
  | ActivePending = 2uy
  | Approved = 3uy
  | Rejected = 4uy

[<CLIMutable>]
type LeaveRequestApprover =
  {
    AbsenceApproverId: int
    //UserId: UserId
    StageNodeId: NodeId
    PersonStatusId: int
    Note: string option
    mutable Status: PersonApproverStatus
  }

[<CLIMutable>]
type PersonStatus =
  {
    PersonStatusId: int
    mutable Note: string option
    StartDate: DateOnly
    EndDate: DateOnly
    StartTime: Nullable<TimeOnly>
    EndTime: Nullable<TimeOnly>
    PersonId: int
    mutable ApprovalStatus: Theasoft.DataClasses.Personal.ts_DataPersStatus.eStatusStatus
    ManualDayCount: Nullable<double>
    DateOfRequest: Nullable<DateOnly>
    ContractId: Nullable<int>
    CompensationWeekValue: Nullable<int>
    ShiftTypeId: int
    Approvers: ICollection<LeaveRequestApprover>
    IsUserRequest: bool
  }
