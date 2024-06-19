import { User } from "../user";
import { RequestApprovalState } from "../requestState";
import { Budget } from "./budget";

export interface Request {
  id: number;
  amount: number;
  invoiceCount: number;
  user: User;
  budget: Budget;
  title: string;
  createDate: Date;
  state: RequestApprovalState;
}
