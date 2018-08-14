import {Component} from '@angular/core';

import {Routes} from '@angular/router';

import {HomeComponent} from './home/home.component';
import {LoginComponent} from './login/login.component';
import {OAuthCallbackHandler} from './login-callback/oauth-callback.guard';
import {OAuthCallbackComponent} from './login-callback/oauth-callback.component';
import {AuthenticationGuard} from "./services/authenticated.guard";
import {UsersComponent} from './users/users.component';
import {UserDetailComponent} from './users/userDetail/userDetail.component';
import {UserListComponent} from './users/userList/userList.component';
import {BudgetsComponent} from './budgets/budgets.component';
import {CategoriesComponent} from './categories/categories.component';
import {CategoryListComponent} from './categories/categoryList/categoryList.component';
import {CategoryDetailComponent} from './categories/categoryDetail/categoryDetail.component';
import {RequestsComponent} from './requests/requests.component';
import {RequestListComponent} from './requests/requestList/requestList.component';
import { UserState } from './model/userState';
import { RequestFilter } from './requests/requestFilter';

export const rootRouterConfig: Routes = [
    {path: '', redirectTo: 'login', pathMatch: 'full'},
    {path: 'home', component: HomeComponent, canActivate: [AuthenticationGuard]},
    {path: 'login', component: LoginComponent},
    {path: 'id_token', component: OAuthCallbackComponent, canActivate: [OAuthCallbackHandler]},
    {path: 'my-budget', component: BudgetsComponent, canActivate: [AuthenticationGuard]},
    {path: 'categories', component: CategoryListComponent, canActivate: [AuthenticationGuard]},
    {path: 'category/:id', component: CategoryDetailComponent, canActivate: [AuthenticationGuard]},
    {
        path: 'requests', component: RequestsComponent, canActivate: [AuthenticationGuard],
        children: [
            {path: '', redirectTo: 'pending', pathMatch: 'full'},
            {path: 'pending', component: RequestListComponent, data: { filter: RequestFilter.Pending }, canActivate: [AuthenticationGuard]},
            {path: 'approved', component: RequestListComponent, data: { filter: RequestFilter.Approved }, canActivate: [AuthenticationGuard]},
            {path: 'rejected', component: RequestListComponent, data: { filter: RequestFilter.Rejected }, canActivate: [AuthenticationGuard]}
        ]
    },
    {
        path: 'users', component: UsersComponent, canActivate: [AuthenticationGuard],
        children: [
            {path: '', redirectTo: 'active', pathMatch: 'full'},
            {path: 'active', component: UserListComponent, data: { filter: UserState.Active }, canActivate: [AuthenticationGuard]},
            {path: 'new', component: UserListComponent, data: { filter: UserState.New }, canActivate: [AuthenticationGuard]},
            {path: 'inactive', component: UserListComponent, data: { filter: UserState.Inactive }, canActivate: [AuthenticationGuard]},
            {path: ':id', component: UserDetailComponent, canActivate: [AuthenticationGuard]}
        ]
    }
];

