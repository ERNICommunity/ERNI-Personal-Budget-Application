import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { UsersComponent } from './users/users.component';
import { UserDetailComponent } from './users/userDetail/userDetail.component';
import { UserListComponent } from './users/userList/userList.component';
import { MyBudgetComponent } from './budgets/myBudget/myBudget.component';
import { RequestsComponent } from './requests/requests.component';
import { RequestListComponent } from './requests/requestList/requestList.component';
import { RequestAddComponent } from './requests/requestAdd/requestAdd.component';
import { UserState } from './model/userState';
import { RequestFilter } from './requests/requestFilter';
import { OtherBudgetsComponent } from './budgets/otherBudgets/otherBudgets.component';
import { OtherBudgetsDetailComponent } from './budgets/otherBudgetsDetail/otherBudgetsDetail.component';
import { RequestEditComponent } from './requests/requestEdit/requestEdit.component';
import { CreateUserComponent } from './users/create-user/create-user.component';
import { RequestMassComponent } from './requests/requestMass/requestMass.component';
import { ViewerGuard } from './services/guards/viewer.guard';
import { NewRequestModalComponent } from './requests/requestAdd/newRequestModal.component';
import { RequestDetailModalComponent } from './requests/requestDetail/requestDetailModal.component';
import { EditRequestModalComponent } from './requests/requestEdit/editRequestModal.component';
import { UserCodesComponent } from './userCodes/user-codes.component';
import { AdminRoleGuard } from './services/guards/admin-role.guard';
import { MsalGuard } from '@azure/msal-angular';
import { UserRoleGuard } from './services/guards/user-role.guard';

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
    {
        path: 'my-budget', canActivate: [MsalGuard, UserRoleGuard],
        children: [
            { path: '', redirectTo: currentYear, pathMatch: 'full' },
            {
                path: ':year', component: MyBudgetComponent, canActivate: [MsalGuard, UserRoleGuard],
                children: [
                    { path: 'request/:state/:id', component: EditRequestModalComponent, canActivate: [MsalGuard, UserRoleGuard] }
                ]
            }
        ]
    },
    { path: 'create-request', component: RequestAddComponent, canActivate: [MsalGuard, UserRoleGuard] },
    {
        path: 'requests', component: RequestsComponent, canActivate: [ViewerGuard],
        children: [
            { path: '', redirectTo: 'pending/' + currentYear, pathMatch: 'full' },
            {
                path: 'pending/:year', component: RequestListComponent, data: { filter: RequestFilter.Pending }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [MsalGuard, UserRoleGuard] },
                ]
            },
            {
                path: 'approved/:year', component: RequestListComponent, data: { filter: RequestFilter.Approved }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [MsalGuard, UserRoleGuard] },
                ]
            },
            {
                path: 'approved-by-superior/:year', component: RequestListComponent, data: { filter: RequestFilter.ApprovedBySuperior }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [MsalGuard, UserRoleGuard] },
                ]
            },
            {
                path: 'rejected/:year', component: RequestListComponent, data: { filter: RequestFilter.Rejected }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [MsalGuard, UserRoleGuard] },
                ]
            }
        ]
    },
    {
        path: 'users', component: UsersComponent, canActivate: [AdminRoleGuard],
        children: [
            { path: '', redirectTo: 'active', pathMatch: 'full' },
            { path: 'active', component: UserListComponent, data: { filter: UserState.Active }, canActivate: [MsalGuard, UserRoleGuard] },
            { path: 'new', component: UserListComponent, data: { filter: UserState.New }, canActivate: [MsalGuard, UserRoleGuard] },
            { path: 'inactive', component: UserListComponent, data: { filter: UserState.Inactive }, canActivate: [MsalGuard, UserRoleGuard] },
            { path: 'detail/:id', component: UserDetailComponent, canActivate: [MsalGuard, UserRoleGuard] },
            { path: 'create', component: CreateUserComponent, canActivate: [MsalGuard, UserRoleGuard] }
        ]
    },
    { path: 'mass-request', component: RequestMassComponent, canActivate: [AdminRoleGuard] },
    { path: 'usercodes', component: UserCodesComponent, canActivate: [MsalGuard, UserRoleGuard] }
];

