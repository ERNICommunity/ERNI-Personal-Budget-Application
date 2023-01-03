import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AdminRoleGuard } from '../services/guards/admin-role.guard';
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
                path: 'other-budgets',
                canActivate: [AdminRoleGuard],
                children: [
                    {
                        path: '',
                        redirectTo: new Date().getFullYear().toString() + '/1',
                        pathMatch: 'full'
                    },
                    {
                        path: 'edit/:id',
                        component: OtherBudgetsDetailComponent,
                        canActivate: [AdminRoleGuard]
                    },
                    {
                        path: ':year/:budgetType',
                        component: OtherBudgetsComponent,
                        canActivate: [AdminRoleGuard]
                    }
                ]
            }
        ])
    ]
})
export class BudgetsModule {}
