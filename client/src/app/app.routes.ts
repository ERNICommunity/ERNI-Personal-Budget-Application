import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { UsersComponent } from './users/users.component';
import { UserDetailComponent } from './users/userDetail/userDetail.component';
import { UserListComponent } from './users/userList/userList.component';
import { RequestAddComponent } from './requests/requestAdd/requestAdd.component';
import { UserState } from './model/userState';
import { OtherBudgetsComponent } from './budgets/otherBudgets/otherBudgets.component';
import { OtherBudgetsDetailComponent } from './budgets/otherBudgetsDetail/otherBudgetsDetail.component';
import { CreateUserComponent } from './users/create-user/create-user.component';
import { RequestMassComponent } from './requests/requestMass/requestMass.component';
import { AdminRoleGuard } from './services/guards/admin-role.guard';
import { MsalGuard } from '@azure/msal-angular';

const currentYear = "2021"; // = (new Date()).getFullYear();

export const rootRouterConfig: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    {
        path: 'other-budgets', canActivate: [AdminRoleGuard],
        children: [
            { path: '', redirectTo: currentYear + "/1", pathMatch: 'full' },
            { path: 'edit/:id', component: OtherBudgetsDetailComponent, canActivate: [AdminRoleGuard] },
            { path: ':year/:budgetType', component: OtherBudgetsComponent, canActivate: [AdminRoleGuard] }
        ]
    },

    { path: 'create-request', component: RequestAddComponent, canActivate: [MsalGuard] },

    {
        path: 'users', component: UsersComponent, canActivate: [AdminRoleGuard],
        children: [
            { path: '', redirectTo: 'active', pathMatch: 'full' },
            { path: 'active', component: UserListComponent, data: { filter: UserState.Active }, canActivate: [MsalGuard] },
            { path: 'new', component: UserListComponent, data: { filter: UserState.New }, canActivate: [MsalGuard] },
            { path: 'inactive', component: UserListComponent, data: { filter: UserState.Inactive }, canActivate: [MsalGuard] },
            { path: 'detail/:id', component: UserDetailComponent, canActivate: [MsalGuard] },
            { path: 'create', component: CreateUserComponent, canActivate: [MsalGuard] }
        ]
    },
    { path: 'mass-request', component: RequestMassComponent, canActivate: [AdminRoleGuard] }
];

