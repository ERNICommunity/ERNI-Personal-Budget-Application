import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TeamRequestModel } from '../../model/request/teamRequestModel';
import { TeamBudgetService } from '../../services/team-budget.service';

@Component({
  selector: 'app-team-request',
  templateUrl: './team-request.component.html',
  styleUrls: ['./team-request.component.css']
})
export class TeamRequestComponent implements OnInit {

  requests: TeamRequestModel[];

  constructor(private route: ActivatedRoute, private teamBudgetService: TeamBudgetService) { }

  async ngOnInit(): Promise<void> {

    this.route.params.subscribe(async params => {
      this.requests = await this.teamBudgetService.getTeamRequests(Number(params['year']));
    });
  }
}
