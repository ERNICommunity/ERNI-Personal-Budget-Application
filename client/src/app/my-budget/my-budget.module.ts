import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MsalGuard } from '@azure/msal-angular';
import { MyBudgetComponent } from './myBudget.component';
import { RouterModule } from '@angular/router';
import { BudgetComponent } from './budget/budget.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';

const currentYear = "2021"; // = (new Date()).getFullYear();

@NgModule({
  declarations: [
    BudgetComponent,
    MyBudgetComponent,
  ],
  imports: [
    CommonModule,
    NgbModule,
    SharedModule,
    RouterModule.forChild([
      {
        path: 'my-budget', canActivate: [MsalGuard],
        children: [
            { path: '', redirectTo: currentYear, pathMatch: 'full' },
            {
                path: ':year', component: MyBudgetComponent, canActivate: [MsalGuard],
                children: [
                    { path: 'request/:state/:id', component: EditRequestModalComponent, canActivate: [MsalGuard] }
                ]
            }
        ]
    },
    ])
  ]
})
export class MyBudgetModule { }
