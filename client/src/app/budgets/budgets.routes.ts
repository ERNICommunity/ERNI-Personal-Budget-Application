import { AdminRoleGuard } from "../services/guards/admin-role.guard";
import { OtherBudgetsDetailComponent } from "./otherBudgetsDetail/otherBudgetsDetail.component";
import { OtherBudgetsComponent } from "./otherBudgets/otherBudgets.component";
import { Routes } from "@angular/router";

export const budgetRoutes = [
  {
    path: "",
    canActivate: [AdminRoleGuard],
    children: [
      {
        path: "",
        redirectTo: new Date().getFullYear().toString() + "/1",
        pathMatch: "full",
      },
      {
        path: "edit/:id",
        component: OtherBudgetsDetailComponent,
        canActivate: [AdminRoleGuard],
      },
      {
        path: ":year/:budgetType",
        component: OtherBudgetsComponent,
        canActivate: [AdminRoleGuard],
      },
    ],
  },
] as Routes;
