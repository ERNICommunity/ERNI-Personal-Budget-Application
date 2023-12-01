import { Routes } from "@angular/router";
import { AuthenticatedComponent } from "./authenticated/authenticated.component";
import { LoginComponent } from "./login/login.component";
import { AutheticatedGuard } from "./services/guards/authenticated.guard";
import { myBudgetRoutes } from "./my-budget/my-budget.routing";
import { budgetRoutes } from "./budgets/budgets.routes";
import { requestsRoutes } from "./requests/requests.module";
import { employeesRoutes } from "./users/users.routes";
import { statisticsRoutes } from "./statistics/statistics.routes";
import { teamBudgetRoutes } from "./team-budget/team-budget.routes";

export const routes: Routes = [
  {
    path: "",
    pathMatch: "full",
    redirectTo: "my-budget",
  },

  {
    path: "",
    component: AuthenticatedComponent,
    canActivate: [AutheticatedGuard],
    children: [
      {
        path: "my-budget",
        children: myBudgetRoutes,
      },
      {
        path: "budgets",
        children: budgetRoutes,
      },
      {
        path: "requests",
        children: requestsRoutes,
      },
      {
        path: "employees",
        children: employeesRoutes,
      },
      {
        path: "statistics",
        children: statisticsRoutes,
      },
      {
        path: "team-budget",
        children: teamBudgetRoutes,
      },
    ],
  },
  { path: "login", component: LoginComponent },
];
