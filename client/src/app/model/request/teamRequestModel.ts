import { RequestApprovalState } from "../requestState";

export interface TransactionModel {
  employeeId: number;
  firstName: string;
  lastName: string;
  amount: number;
  isSubordinate: boolean;
}

export interface TeamRequestModel {
  id: number;
  transactions: TransactionModel[];
  title: string;
  state: RequestApprovalState;
  createDate: Date;
  totalAmount: number;
}
