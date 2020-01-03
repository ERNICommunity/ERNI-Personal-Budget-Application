import {User} from "../user";
import {Category} from "../category";
import {RequestState} from "../requestState";
import { Budget } from "./budget";

export class Request {
    id:number;
    amount: number;
    user: User;
    category: Category;
    budget: Budget;
    categoryId : number;
    title: string;
    date: Date;
    state: RequestState;
    url : string;
}