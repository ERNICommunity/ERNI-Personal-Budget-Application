import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeamBudgetComponent } from './team-budget.component';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TeamBudgetStateComponent } from './team-budget-state/team-budget-state.component';
import { TeamRequestComponent } from './team-request/team-request.component';
import { CreateRequestComponent } from './create-request/create-request.component';
import { InputNumberModule } from 'primeng/inputnumber';
import { SharedModule } from '../shared/shared.module';
import { superiorGuard } from '../services/guards/superior.guard';

@NgModule({
  declarations: [TeamBudgetComponent, TeamBudgetStateComponent, TeamRequestComponent, CreateRequestComponent],
  imports: [
    SharedModule,
    FormsModule,
    CommonModule,
    RouterModule,
    InputNumberModule,
    RouterModule.forChild([
      {
        path: '',
        canActivate: [superiorGuard],
        children: [
          {
            path: '',
            redirectTo: new Date().getFullYear().toString(),
            pathMatch: 'full',
          },
          {
            path: ':year',
            component: TeamBudgetComponent,
            children: [
              {
                path: 'create-request',
                component: CreateRequestComponent,
              },
              {
                path: 'request/:requestId',
                component: CreateRequestComponent,
              },
            ],
          },
        ],
      },
    ]),
  ],
  exports: [TeamBudgetComponent],
})
export class TeamBudgetModule {}
