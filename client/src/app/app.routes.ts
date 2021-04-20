import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RequestAddComponent } from './requests/requestAdd/requestAdd.component';
import { OtherBudgetsComponent } from './budgets/otherBudgets/otherBudgets.component';
import { OtherBudgetsDetailComponent } from './budgets/otherBudgetsDetail/otherBudgetsDetail.component';
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
    { path: 'mass-request', component: RequestMassComponent, canActivate: [AdminRoleGuard] }
];

