import {User} from "./user";
import {Category} from "./category";
import {RequestState} from "./requestState";

export class RequestMass {
    id:number;
    amount: number;
    users: User[];
    category: Category;
    title: string;
    date: Date;
    state: RequestState;
    url : string;
}