import { BudgetType } from "../budgetType";
import { TeamRequests } from "./team-requests";

export class TeamBudget {
    year: number;
    amount: number;
    amountLeft: number;
    title: string;
    type: BudgetType;
    teamRequests: TeamRequests[];
}
