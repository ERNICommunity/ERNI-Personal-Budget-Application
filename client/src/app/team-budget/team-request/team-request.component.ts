import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { combineLatest } from 'rxjs';
import { TeamRequestModel } from '../../model/request/teamRequestModel';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { TeamBudgetService } from '../../services/team-budget.service';

@Component({
  selector: 'app-team-request',
  templateUrl: './team-request.component.html',
})
export class TeamRequestComponent implements OnInit {
  requests: TeamRequestModel[];

  constructor(
    private route: ActivatedRoute,
    private teamBudgetService: TeamBudgetService,
    private notificationService: DataChangeNotificationService,
  ) {}

  async ngOnInit(): Promise<void> {
    combineLatest([this.route.params, this.notificationService.notifications$]).subscribe(async ([params]) => {
      this.requests = await this.teamBudgetService.getTeamRequests(Number(params['year']));
    });
  }
}
