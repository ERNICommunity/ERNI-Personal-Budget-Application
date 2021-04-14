import { User } from "../user";
import { RequestApprovalState } from "../requestState";
import { Budget } from "./budget";

export class Request {
    id: number;
    amount: number;
    user: User;
    budget: Budget;
    title: string;
    date: Date;
    createDate: Date;
    state: RequestApprovalState;
}