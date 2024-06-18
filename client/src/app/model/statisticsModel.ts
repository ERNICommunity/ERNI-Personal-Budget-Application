import { BudgetTypeEnum } from "./budgetTypeEnum";

export class BudgetStatisticsModel {
  budgetType: BudgetTypeEnum;
  budgetCount: number;

  totalAmount: number;
  totalSpentAmount: number;
}

export class StatisticsModel {
  budgets: BudgetStatisticsModel[];
}
