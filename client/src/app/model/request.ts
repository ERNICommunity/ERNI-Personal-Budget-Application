import {User} from "./user";
import {Category} from "./category";

export class Request {
    id:number;
    amount: number;
    userId: number;
    categoryId: number
    title: string;
    date: Date;
}