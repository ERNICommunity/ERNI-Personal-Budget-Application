import { Routes } from "@angular/router";
import { AuthenticatedComponent } from "./authenticated/authenticated.component";
import { LoginComponent } from "./login/login.component";
import { RequestMassComponent } from "./requests/requestMass/requestMass.component";
import { AdminRoleGuard } from "./services/guards/admin-role.guard";
import { AutheticatedGuard } from "./services/guards/authenticated.guard";

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
        loadChildren: () =>
          import("./my-budget/my-budget.module").then((m) => m.MyBudgetModule),
      },
      {
        path: "budgets",
        loadChildren: () =>
          import("./budgets/budgets.module").then((m) => m.BudgetsModule),
      },
      {
        path: "requests",
        loadChildren: () =>
          import("./requests/requests.module").then((m) => m.RequestsModule),
      },
      {
        path: "employees",
        loadChildren: () =>
          import("./users/users.module").then((m) => m.UsersModule),
      },
      {
        path: "statistics",
        loadChildren: () =>
          import("./statistics/statistics.module").then(
            (m) => m.StatisticsModule
          ),
      },
      {
        path: "team-budget",
        loadChildren: () =>
          import("./team-budget/team-budget.module").then(
            (m) => m.TeamBudgetModule
          ),
      },
    ],
  },
  { path: "login", component: LoginComponent },
];
