import { User } from "../user";
import { RequestState } from "../requestState";
import { Budget } from "./budget";
import { Transaction } from "./transaction";

export class Request {
    id: number;
    amount: number;
    user: User;
    budget: Budget;
    title: string;
    date: Date;
    state: RequestState;
    transactions: Transaction[];
}