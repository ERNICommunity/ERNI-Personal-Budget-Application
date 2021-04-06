import { Component, OnInit } from '@angular/core';
import { TeamBudgetModel } from '../../model/teamBudget';
import { TeamBudgetService } from '../../services/team-budget.service';

@Component({
  selector: 'app-team-budget-state',
  templateUrl: './team-budget-state.component.html',
  styleUrls: ['./team-budget-state.component.css']
})
export class TeamBudgetStateComponent implements OnInit {

  teamBudgets: TeamBudgetModel[];

  constructor(private teamBudgetService: TeamBudgetService) { }

  async ngOnInit(): Promise<void> {
    this.teamBudgets = await this.teamBudgetService.getDefaultTeamBudget((new Date()).getFullYear());
  }
}
