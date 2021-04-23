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
import { CreateRequestComponent } from './create-request/create-request.component';
import { PickListModule } from 'primeng/picklist';
import {InputNumberModule} from 'primeng/inputnumber';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [
    TeamBudgetComponent,
    TeamBudgetStateComponent,
    TeamRequestComponent,
    CreateRequestComponent,
  ],
  imports: [
    NgbModule,
    SharedModule,
    FormsModule,
    CommonModule,
    RouterModule,
    InputNumberModule,
    PickListModule
  ],
  exports: [
    TeamBudgetComponent
  ]
})
export class TeamBudgetModule { }
