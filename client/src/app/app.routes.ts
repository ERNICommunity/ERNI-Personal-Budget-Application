import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RequestAddComponent } from './requests/requestAdd/requestAdd.component';
import { RequestMassComponent } from './requests/requestMass/requestMass.component';
import { AdminRoleGuard } from './services/guards/admin-role.guard';
import { MsalGuard } from '@azure/msal-angular';

export const rootRouterConfig: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'create-request', component: RequestAddComponent, canActivate: [MsalGuard] },
    { path: 'mass-request', component: RequestMassComponent, canActivate: [AdminRoleGuard] }
];

