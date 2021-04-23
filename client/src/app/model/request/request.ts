import { User } from "../user";
import { RequestApprovalState } from "../requestState";
import { Budget } from "./budget";

export class Request {
    id: number;
    amount: number;
    invoicedAmount?: number;
    invoiceCount: number;
    user: User;
    budget: Budget;
    title: string;
    date: Date;
    createDate: Date;
    state: RequestApprovalState;
}
