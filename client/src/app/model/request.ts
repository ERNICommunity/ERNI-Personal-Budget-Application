import {User} from "./user";
import {Category} from "./category";

export class Request {
    id:number;
    amount: number;
    user: User;
    category: Category
    title: string;
    date: Date;
}