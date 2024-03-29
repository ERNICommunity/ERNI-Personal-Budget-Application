import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MsalGuard } from '@azure/msal-angular';
import { MyBudgetComponent } from './myBudget.component';
import { RouterModule } from '@angular/router';
import { BudgetComponent } from './budget/budget.component';
import { SharedModule } from '../shared/shared.module';
import { RequestEditComponent } from '../requests/requestEdit/requestEdit.component';

@NgModule({
    declarations: [BudgetComponent, MyBudgetComponent],
    imports: [
        CommonModule,
        SharedModule,
        RouterModule.forChild([
            {
                path: 'my-budget',
                canActivate: [MsalGuard],
                children: [
                    {
                        path: '',
                        redirectTo: new Date().getFullYear().toString(),
                        pathMatch: 'full'
                    },
                    {
                        path: ':year',
                        component: MyBudgetComponent,
                        canActivate: [MsalGuard],
                        children: [
                            {
                                path: 'new-request/:budgetId',
                                component: RequestEditComponent,
                                canActivate: [MsalGuard]
                            },
                            {
                                path: 'request/:requestId',
                                component: RequestEditComponent,
                                canActivate: [MsalGuard]
                            }
                        ]
                    }
                ]
            }
        ])
    ]
})
export class MyBudgetModule {}
