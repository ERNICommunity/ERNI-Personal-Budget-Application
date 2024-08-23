import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { adminGuard } from '../services/guards/admin.guard';
import { OtherBudgetsDetailComponent } from './otherBudgetsDetail/otherBudgetsDetail.component';
import { OtherBudgetsComponent } from './otherBudgets/otherBudgets.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { CreateBudgetsComponent } from './create-budgets/create-budgets.component';

@NgModule({
  declarations: [OtherBudgetsComponent, OtherBudgetsDetailComponent, CreateBudgetsComponent],
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    RouterModule.forChild([
      {
        path: '',
        canActivate: [adminGuard],
        children: [
          {
            path: '',
            redirectTo: new Date().getFullYear().toString() + '/1',
            pathMatch: 'full',
          },
          {
            path: 'edit/:id',
            component: OtherBudgetsDetailComponent,
          },
          {
            path: ':year/:budgetType',
            component: OtherBudgetsComponent,
          },
        ],
      },
    ]),
  ],
})
export class BudgetsModule {}
