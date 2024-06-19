export interface EmployeeModel {
  id: number;
  firstName: string;
  lastName: string;
  isTeamMember: boolean;
}

export interface TeamBudgetModel {
  employee: EmployeeModel;
  budgetTotal: number;
  budgetLeft: number;
  budgetSpent: number;
}
