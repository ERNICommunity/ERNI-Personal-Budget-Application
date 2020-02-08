import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { OAuthCallbackHandler } from './login-callback/oauth-callback.guard';
import { OAuthCallbackComponent } from './login-callback/oauth-callback.component';
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
import { AdminGuard } from './services/guards/admin.guard';
import { ViewerGuard } from './services/guards/viewer.guard';
import { AuthenticationGuard } from './services/guards/authentication.guard';
import { NewRequestModalComponent } from './requests/requestAdd/newRequestModal.component';
import { RequestDetailModalComponent } from './requests/requestDetail/requestDetailModal.component';
import { NewTeamRequestModalComponent } from './requests/new-team-request-modal/new-team-request-modal.component';
import { TeamRequestAddComponent } from './requests/team-request-add/team-request-add.component';
import { EditRequestModalComponent } from './requests/requestEdit/editRequestModal.component';

const currentYear = "2020"; // = (new Date()).getFullYear();

export const rootRouterConfig: Routes = [
    { path: '', redirectTo: 'my-budget', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'id_token', component: OAuthCallbackComponent, canActivate: [OAuthCallbackHandler] },
    {
        path: 'other-budgets', canActivate: [AdminGuard],
        children: [
            { path: '', redirectTo: currentYear + "/1", pathMatch: 'full' },
            { path: ':year/:budgetType', component: OtherBudgetsComponent, canActivate: [AdminGuard] },
            { path: ':year/edit/:id', component: OtherBudgetsDetailComponent, canActivate: [AdminGuard] }
        ]
    },
    {
        path: 'my-budget', canActivate: [AuthenticationGuard],
        children: [
            { path: '', redirectTo: currentYear, pathMatch: 'full' },
            {
                path: ':year', component: MyBudgetComponent, canActivate: [AuthenticationGuard],
                children: [
                    { path: 'create-request/:budgetId', component: NewRequestModalComponent, canActivate: [AuthenticationGuard] },
                    { path: 'create-team-request/:year', component: NewTeamRequestModalComponent, canActivate: [AuthenticationGuard] },
                    { path: 'request/:requestId', component: RequestDetailModalComponent, canActivate: [AuthenticationGuard] },
                    { path: 'request/:requestId/edit', component: EditRequestModalComponent, canActivate: [AuthenticationGuard] },
                    { path: 'request/:requestId/edit/:type', component: EditRequestModalComponent, canActivate: [AuthenticationGuard] },
                ]
            }
        ]
    },
    { path: 'create-request', component: RequestAddComponent, canActivate: [AuthenticationGuard] },
    { path: 'create-team-request', component: TeamRequestAddComponent, canActivate: [AuthenticationGuard] },
    {
        path: 'requests', component: RequestsComponent, canActivate: [ViewerGuard],
        children: [
            { path: '', redirectTo: 'pending/' + currentYear, pathMatch: 'full' },
            {
                path: 'pending/:year', component: RequestListComponent, data: { filter: RequestFilter.Pending }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId/type/:type', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [AuthenticationGuard] },
                ]
            },
            {
                path: 'approved/:year', component: RequestListComponent, data: { filter: RequestFilter.Approved }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId/type/:type', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [AuthenticationGuard] },
                ]
            },
            {
                path: 'approved-by-superior/:year', component: RequestListComponent, data: { filter: RequestFilter.ApprovedBySuperior }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId/type/:type', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [AuthenticationGuard] },
                ]
            },
            {
                path: 'rejected/:year', component: RequestListComponent, data: { filter: RequestFilter.Rejected }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId/type/:type', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [AuthenticationGuard] },
                ]
            }
        ]
    },
    {
        path: 'users', component: UsersComponent, canActivate: [AdminGuard],
        children: [
            { path: '', redirectTo: 'active', pathMatch: 'full' },
            { path: 'active', component: UserListComponent, data: { filter: UserState.Active }, canActivate: [AuthenticationGuard] },
            { path: 'new', component: UserListComponent, data: { filter: UserState.New }, canActivate: [AuthenticationGuard] },
            { path: 'inactive', component: UserListComponent, data: { filter: UserState.Inactive }, canActivate: [AuthenticationGuard] },
            { path: 'detail/:id', component: UserDetailComponent, canActivate: [AuthenticationGuard] },
            { path: 'create', component: CreateUserComponent, canActivate: [AuthenticationGuard] }
        ]
    },
    { path: 'request-mass', component: RequestMassComponent, canActivate: [AdminGuard] }
];

