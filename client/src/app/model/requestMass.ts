import { User } from "./user";
import { RequestApprovalState } from "./requestState";

export class RequestMass {
    id: number;
    amount: number;
    users: User[];
    title: string;
    date: Date;
    state: RequestApprovalState;
}