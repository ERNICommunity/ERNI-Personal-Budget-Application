import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { combineLatest } from "rxjs";
import { TeamBudgetModel } from "../../model/teamBudget";
import { DataChangeNotificationService } from "../../services/dataChangeNotification.service";
import { TeamBudgetService } from "../../services/team-budget.service";
import { TableModule } from "primeng/table";
import { ButtonModule } from "primeng/button";
import { SharedModule } from "../../shared/shared.module";

@Component({
  selector: "app-team-budget-state",
  templateUrl: "./team-budget-state.component.html",
  styleUrls: ["./team-budget-state.component.css"],
  standalone: true,
  imports: [SharedModule, ButtonModule, RouterLink, TableModule],
})
export class TeamBudgetStateComponent implements OnInit {
  teamBudgets: TeamBudgetModel[] = [];

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
      var yearParam = params["year"];

      this.teamBudgets = await this.teamBudgetService.getDefaultTeamBudget(
        yearParam
      );
    });
  }
}
