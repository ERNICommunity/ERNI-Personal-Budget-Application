import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RequestMassComponent } from './requests/requestMass/requestMass.component';
import { AdminRoleGuard } from './services/guards/admin-role.guard';

export const rootRouterConfig: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'mass-request', component: RequestMassComponent, canActivate: [AdminRoleGuard] }
];

