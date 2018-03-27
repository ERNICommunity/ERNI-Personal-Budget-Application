import { Component } from '@angular/core';

import { Routes } from '@angular/router';

import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { OAuthCallbackHandler } from './login-callback/oauth-callback.guard';
import { OAuthCallbackComponent } from './login-callback/oauth-callback.component';
import { AuthenticationGuard } from "./services/authenticated.guard";
import { RequestsComponent } from './requests/requests.component';
import { UsersComponent } from './users/users.component';

export const rootRouterConfig: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'home', component: HomeComponent, canActivate: [AuthenticationGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'id_token', component: OAuthCallbackComponent, canActivate: [OAuthCallbackHandler] },
  { path: 'requests', component: RequestsComponent, canActivate: [AuthenticationGuard] },
  { path: 'users', component: UsersComponent, canActivate: [AuthenticationGuard] }
];

