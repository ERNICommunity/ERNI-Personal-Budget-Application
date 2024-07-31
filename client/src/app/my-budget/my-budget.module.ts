import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MyBudgetComponent } from './myBudget.component';
import { RouterModule } from '@angular/router';
import { BudgetComponent } from './budget/budget.component';
import { SharedModule } from '../shared/shared.module';
import { RequestEditComponent } from '../requests/requestEdit/requestEdit.component';
import { MoneyConfettiDirective } from "../shared/directives/money-confetti.directive";

@NgModule({
    declarations: [BudgetComponent, MyBudgetComponent],
    imports: [
        CommonModule,
        SharedModule,
        RouterModule.forChild([
            {
                path: '',
                redirectTo: new Date().getFullYear().toString(),
                pathMatch: 'full'
            },
            {
                path: ':year',
                title: (route) => `My Budget - ${route.paramMap.get('year')} | PBA`,
                component: MyBudgetComponent,
                children: [
                    {
                        path: 'new-request/:budgetId',
                        component: RequestEditComponent
                    },
                    {
                        path: 'request/:requestId',
                        component: RequestEditComponent
                    }
                ]
            }
        ]),
        MoneyConfettiDirective
    ]
})
export class MyBudgetModule {}
