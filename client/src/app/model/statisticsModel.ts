import { BudgetTypeEnum } from "./budgetTypeEnum";

export interface BudgetStatisticsModel {
  budgetType: BudgetTypeEnum;
  budgetCount: number;

  totalAmount: number;
  totalSpentAmount: number;
}

export interface StatisticsModel {
  budgets: BudgetStatisticsModel;
}
