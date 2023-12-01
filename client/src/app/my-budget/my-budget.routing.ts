import { MyBudgetComponent } from "./myBudget.component";
import { RequestEditComponent } from "../requests/requestEdit/requestEdit.component";
import { Routes } from "@angular/router";

export const myBudgetRoutes = [
  {
    path: "",
    redirectTo: new Date().getFullYear().toString(),
    pathMatch: "full",
  },
  {
    path: ":year",
    component: MyBudgetComponent,
    children: [
      {
        path: "new-request/:budgetId",
        component: RequestEditComponent,
      },
      {
        path: "request/:requestId",
        component: RequestEditComponent,
      },
    ],
  },
] as Routes;
