export class EmployeeModel {
  id: number;
  firstName: string;
  lastName: string;
  isTeamMember: boolean;
}

export class TeamBudgetModel {
  employee: EmployeeModel;
  budgetTotal: number;
  budgetLeft: number;
}
