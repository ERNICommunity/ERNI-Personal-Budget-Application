import { RequestState } from "../requestState";

export class TeamRequests {
    id: number;
    userId: number;
    title: string;
    amount: number;
    date: Date;
    state: RequestState;
    Requests: Request[];
}
