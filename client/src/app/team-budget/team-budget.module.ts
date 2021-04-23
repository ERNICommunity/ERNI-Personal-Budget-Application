import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeamBudgetComponent } from './team-budget.component';
import { BrowserModule } from '@angular/platform-browser';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { TeamBudgetStateComponent } from './team-budget-state/team-budget-state.component';
import { TeamRequestComponent } from './team-request/team-request.component';



@NgModule({
  declarations: [
    TeamBudgetComponent,
    TeamBudgetStateComponent,
    TeamRequestComponent
  ],
  imports: [
    NgbModule,
    CommonModule,
    RouterModule
  ],
  exports: [
    TeamBudgetComponent
  ]
})
export class TeamBudgetModule { }
