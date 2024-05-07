import { User } from './user';
import { Request } from './request/request';
import { BudgetTypeEnum } from './budgetTypeEnum';

export class CurrentUsersBudget {
    id: number;
    year: number;
    amount: number;
    amountLeft: number;
    isEditable: boolean;
    user: User;
    title: string;
    type: BudgetTypeEnum;
    requests: Request[];
}
