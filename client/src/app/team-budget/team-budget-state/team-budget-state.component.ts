import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { combineLatest } from 'rxjs';
import { TeamBudgetModel } from '../../model/teamBudget';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { TeamBudgetService } from '../../services/team-budget.service';

@Component({
  selector: 'app-team-budget-state',
  templateUrl: './team-budget-state.component.html',
  styleUrls: ['./team-budget-state.component.css'],
})
export class TeamBudgetStateComponent implements OnInit {
  teamBudgets: TeamBudgetModel[];

  constructor(
    private route: ActivatedRoute,
    private teamBudgetService: TeamBudgetService,
    private dataChangeNotificationService: DataChangeNotificationService
  ) {}

  async ngOnInit(): Promise<void> {
    combineLatest([
      this.route.params,
      this.dataChangeNotificationService.notifications$,
    ]).subscribe(async ([params]) => {
      const yearParam = params['year'];

      this.teamBudgets = await this.teamBudgetService.getDefaultTeamBudget(
        yearParam
      );
    });
  }
}
