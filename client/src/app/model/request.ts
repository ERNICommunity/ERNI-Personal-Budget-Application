import {User} from "./user";
export class Request {
    id:number;
    amount: number;
    user: User;
    category: string
    title: string;
    date: Date;
}