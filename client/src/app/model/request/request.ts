import {User} from "../user";
import {RequestState} from "../requestState";
import { Budget } from "./budget";

export class Request {
    id:number;
    amount: number;
    user: User;
    budget: Budget;
    title: string;
    date: Date;
    state: RequestState;
}