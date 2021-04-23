import { Component, OnInit } from '@angular/core';
import { TeamRequestModel } from '../../model/request/teamRequestModel';
import { TeamBudgetService } from '../../services/team-budget.service';

@Component({
  selector: 'app-team-request',
  templateUrl: './team-request.component.html',
  styleUrls: ['./team-request.component.css']
})
export class TeamRequestComponent implements OnInit {

  requests: TeamRequestModel[];

  constructor(private teamBudgetService: TeamBudgetService) { }

  async ngOnInit(): Promise<void> {
    this.requests = await this.teamBudgetService.getTeamRequets((new Date()).getFullYear());
  }
}
