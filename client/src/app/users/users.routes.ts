import { UsersComponent } from "./users.component";
import { UserListComponent } from "./userList/userList.component";
import { UserDetailComponent } from "./userDetail/userDetail.component";
import { CreateUserComponent } from "./create-user/create-user.component";
import { Routes } from "@angular/router";
import { AdminRoleGuard } from "../services/guards/admin-role.guard";
import { UserState } from "../model/userState";

export const employeesRoutes = [
  {
    path: "",
    component: UsersComponent,
    canActivate: [AdminRoleGuard],
    children: [
      { path: "", redirectTo: "active", pathMatch: "full" },
      {
        path: "active",
        component: UserListComponent,
        data: { filter: UserState.Active },
      },
      {
        path: "new",
        component: UserListComponent,
        data: { filter: UserState.New },
      },
      {
        path: "inactive",
        component: UserListComponent,
        data: { filter: UserState.Inactive },
      },
      {
        path: "detail/:id",
        component: UserDetailComponent,
      },
      {
        path: "create",
        component: CreateUserComponent,
      },
    ],
  },
] as Routes;
