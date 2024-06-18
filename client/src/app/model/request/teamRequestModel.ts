import { RequestApprovalState } from '../requestState';

export class TransactionModel {
  employeeId: number;
  firstName: string;
  lastName: string;
  amount: number;
  isSubordinate: boolean;
}

export class TeamRequestModel {
  id: number;
  transactions: TransactionModel[];
  title: string;
  state: RequestApprovalState;
  createDate: Date;
  totalAmount: number;
}
