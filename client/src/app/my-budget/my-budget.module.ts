import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MsalGuard } from '@azure/msal-angular';
import { MyBudgetComponent } from './myBudget.component';
import { RouterModule } from '@angular/router';
import { BudgetComponent } from './budget/budget.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { RequestEditComponent } from '../requests/requestEdit/requestEdit.component';

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
                { path: 'new-request/:budgetId', component: RequestEditComponent, canActivate: [MsalGuard] },
                { path: 'request/:requestId', component: RequestEditComponent, canActivate: [MsalGuard] }
              ]
            }
        ]
    },
    ])
  ]
})
export class MyBudgetModule { }
