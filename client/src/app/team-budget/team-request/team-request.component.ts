import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { combineLatest } from 'rxjs';
import { TeamRequestModel } from '../../model/request/teamRequestModel';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { TeamBudgetService } from '../../services/team-budget.service';
import { TableModule } from 'primeng/table';
import { SharedModule } from 'primeng/api';
import { PanelModule } from 'primeng/panel';
import { NgFor, NgClass } from '@angular/common';

@Component({
    selector: 'app-team-request',
    templateUrl: './team-request.component.html',
    styleUrls: ['./team-request.component.css'],
    standalone: true,
    imports: [NgFor, PanelModule, SharedModule, RouterLink, TableModule, NgClass]
})
export class TeamRequestComponent implements OnInit {

  requests: TeamRequestModel[];

  constructor(private route: ActivatedRoute, private teamBudgetService: TeamBudgetService, private notificationService: DataChangeNotificationService) { }

  async ngOnInit(): Promise<void> {
    combineLatest([ this.route.params, this.notificationService.notifications$]).subscribe(async ([params, _]) => {
      this.requests = await this.teamBudgetService.getTeamRequests(Number(params['year']));
    });
  }
}
