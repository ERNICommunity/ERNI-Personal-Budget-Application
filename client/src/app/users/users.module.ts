import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsersComponent } from './users.component';
import { UserListComponent } from './userList/userList.component';
import { UserDetailComponent } from './userDetail/userDetail.component';
import { CreateUserComponent } from './create-user/create-user.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AdminRoleGuard } from '../services/guards/admin-role.guard';
import { UserState } from '../model/userState';
import { MsalGuard } from '@azure/msal-angular';
import { SharedModule } from '../shared/shared.module';

@NgModule({
    declarations: [
        UsersComponent,
        UserListComponent,
        UserDetailComponent,
        CreateUserComponent
    ],
    imports: [
        FormsModule,
        ReactiveFormsModule,
        CommonModule,
        SharedModule,
        RouterModule.forChild([
            {
                path: 'users',
                component: UsersComponent,
                canActivate: [AdminRoleGuard],
                children: [
                    { path: '', redirectTo: 'active', pathMatch: 'full' },
                    {
                        path: 'active',
                        component: UserListComponent,
                        data: { filter: UserState.Active },
                        canActivate: [MsalGuard]
                    },
                    {
                        path: 'new',
                        component: UserListComponent,
                        data: { filter: UserState.New },
                        canActivate: [MsalGuard]
                    },
                    {
                        path: 'inactive',
                        component: UserListComponent,
                        data: { filter: UserState.Inactive },
                        canActivate: [MsalGuard]
                    },
                    {
                        path: 'detail/:id',
                        component: UserDetailComponent,
                        canActivate: [MsalGuard]
                    },
                    {
                        path: 'create',
                        component: CreateUserComponent,
                        canActivate: [MsalGuard]
                    }
                ]
            }
        ])
    ]
})
export class UsersModule {}
