import { User } from "./user";
import { RequestState } from "./requestState";

export class RequestMass {
    id: number;
    amount: number;
    users: User[];
    title: string;
    date: Date;
    state: RequestState;
}