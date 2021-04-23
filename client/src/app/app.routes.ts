import { Routes } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { LoginComponent } from './login/login.component';
import { RequestMassComponent } from './requests/requestMass/requestMass.component';
import { AdminRoleGuard } from './services/guards/admin-role.guard';
import { TeamBudgetComponent } from './team-budget/team-budget.component';

export const rootRouterConfig: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'mass-request', component: RequestMassComponent, canActivate: [AdminRoleGuard] },
    {
      path: 'team-budget', canActivate: [MsalGuard],
      children: [
          { path: '', redirectTo: new Date().getFullYear().toString(), pathMatch: 'full' },
          {
              path: ':year', component: TeamBudgetComponent, canActivate: [MsalGuard],
              children: [
                  // { path: 'create-request/:budgetId', component: NewRequestModalComponent, canActivate: [MsalGuard] },
                  // { path: 'request/:requestId', component: RequestDetailModalComponent, canActivate: [MsalGuard] },
                  // { path: 'request/:requestId/edit', component: EditRequestModalComponent, canActivate: [MsalGuard] }
              ]
          }
      ]
    },
];

