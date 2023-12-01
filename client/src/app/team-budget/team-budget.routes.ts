import { TeamBudgetComponent } from "./team-budget.component";
import { Routes } from "@angular/router";
import { CreateRequestComponent } from "./create-request/create-request.component";
import { SuperiorGuard } from "../services/guards/superior.guard";

export const teamBudgetRoutes = [
  {
    path: "",
    canActivate: [SuperiorGuard],
    children: [
      {
        path: "",
        redirectTo: new Date().getFullYear().toString(),
        pathMatch: "full",
      },
      {
        path: ":year",
        component: TeamBudgetComponent,
        children: [
          {
            path: "create-request",
            component: CreateRequestComponent,
          },
          {
            path: "request/:requestId",
            component: CreateRequestComponent,
          },
        ],
      },
    ],
  },
] as Routes;
