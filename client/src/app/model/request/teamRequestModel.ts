import { RequestApprovalState } from "../requestState";

export class TransactionModel {
  firstName: string;
  lastName: string;
  amount: number;
  isSubordinate: boolean;
}

export class TeamRequestModel {
  transactions: TransactionModel[];
  title: string;
  state: RequestApprovalState;
  createDate: Date;
  totalAmount: number;
}
