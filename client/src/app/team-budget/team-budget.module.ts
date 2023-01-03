import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeamBudgetComponent } from './team-budget.component';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { TeamBudgetStateComponent } from './team-budget-state/team-budget-state.component';
import { TeamRequestComponent } from './team-request/team-request.component';
import { CreateRequestComponent } from './create-request/create-request.component';
import { PickListModule } from 'primeng/picklist';
import { InputNumberModule } from 'primeng/inputnumber';
import { SharedModule } from '../shared/shared.module';
import { MsalGuard } from '@azure/msal-angular';

@NgModule({
    declarations: [
        TeamBudgetComponent,
        TeamBudgetStateComponent,
        TeamRequestComponent,
        CreateRequestComponent
    ],
    imports: [
        SharedModule,
        FormsModule,
        CommonModule,
        RouterModule,
        InputNumberModule,
        PickListModule,
        RouterModule.forChild([
            {
                path: 'team-budget',
                canActivate: [MsalGuard],
                children: [
                    {
                        path: '',
                        redirectTo: new Date().getFullYear().toString(),
                        pathMatch: 'full'
                    },
                    {
                        path: ':year',
                        component: TeamBudgetComponent,
                        canActivate: [MsalGuard],
                        children: [
                            {
                                path: 'create-request',
                                component: CreateRequestComponent,
                                canActivate: [MsalGuard]
                            },
                            {
                                path: 'request/:requestId',
                                component: CreateRequestComponent,
                                canActivate: [MsalGuard]
                            }
                            // { path: 'request/:requestId/edit', component: EditRequestModalComponent, canActivate: [MsalGuard] }
                        ]
                    }
                ]
            }
        ])
    ],
    exports: [TeamBudgetComponent]
})
export class TeamBudgetModule {}
