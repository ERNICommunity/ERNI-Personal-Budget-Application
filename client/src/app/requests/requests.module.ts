import { Routes } from "@angular/router";
import { RequestsComponent } from "./requests.component";
import { ViewerGuard } from "../services/guards/viewer.guard";
import { RequestDetailComponent } from "./requestDetail/requestDetail.component";
import { RequestEditComponent } from "./requestEdit/requestEdit.component";
import { RequestListComponent } from "./requestList/requestList.component";
import { RequestMassComponent } from "./requestMass/requestMass.component";
import { AdminRoleGuard } from "../services/guards/admin-role.guard";

export const requestsRoutes = [
  {
    path: "",
    component: RequestsComponent,
    canActivate: [ViewerGuard],
    children: [
      {
        path: "",
        redirectTo: "1/pending/" + new Date().getFullYear().toString(),
        pathMatch: "full",
      },
      {
        path: ":budgetType/:requestState/:year",
        component: RequestListComponent,
        canActivate: [ViewerGuard],
        children: [
          {
            path: "detail/:requestId",
            component: RequestDetailComponent,
            canActivate: [ViewerGuard],
          },
          {
            path: "edit/:id",
            component: RequestEditComponent,
          },
        ],
      },
      {
        path: "mass-request",
        component: RequestMassComponent,
        canActivate: [AdminRoleGuard],
      },
    ],
  },
] as Routes;
