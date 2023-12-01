import { Routes } from "@angular/router";
import { AdminRoleGuard } from "../services/guards/admin-role.guard";
import { StatisticsComponent } from "./statistics/statistics.component";

export const statisticsRoutes = [
  {
    path: "",
    redirectTo: new Date().getFullYear().toString(),
    pathMatch: "full",
  },
  {
    path: ":year",
    component: StatisticsComponent,
    canActivate: [AdminRoleGuard],
  },
] as Routes;
