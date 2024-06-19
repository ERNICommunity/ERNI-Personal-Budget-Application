import { BudgetType } from "../budgetType";
export interface Budget {
  id: number;
  title: string;
  type: BudgetType;
}
