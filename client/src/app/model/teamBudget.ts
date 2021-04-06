export class EmployeeModel {
  id: number;
  firstName: string;
  lastName: string;
}

export class TeamBudgetModel {
  employee: EmployeeModel;
  budgetTotal: number;
  budgetLeft: number;
}
