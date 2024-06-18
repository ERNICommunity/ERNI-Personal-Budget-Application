import { User } from './user';
import { Request } from './request/request';
import { BudgetTypeEnum } from './budgetTypeEnum';

export class Budget {
  id: number;
  year: number;
  amount: number;
  amountLeft: number;
  user: User;
  title: string;
  type: BudgetTypeEnum;
  requests: Request[];
}
