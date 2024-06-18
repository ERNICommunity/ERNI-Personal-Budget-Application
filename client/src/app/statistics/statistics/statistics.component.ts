import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { MenuItem } from "primeng/api/menuitem";
import { StatisticsModel } from "../../model/statisticsModel";
import { ConfigService } from "../../services/config.service";
import { StatisticsService } from "../../services/statistics.service";

@Component({
  selector: "app-statistics",
  templateUrl: "./statistics.component.html",
  styleUrls: ["./statistics.component.css"],
})
export class StatisticsComponent implements OnInit {
  years: MenuItem[];
  selectedYear: MenuItem;
  currentYear: number;

  statistics: StatisticsModel;

  constructor(
    config: ConfigService,
    private statisticsService: StatisticsService,
    private activatedRoute: ActivatedRoute
  ) {
    this.years = [];
    this.currentYear = new Date().getFullYear();
    for (let year = this.currentYear; year >= config.getOldestYear; year--) {
      this.years.push({
        label: year.toString(),
        routerLink: ["/statistics", year],
      });
    }
  }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(async (params) => {
      this.statistics = await this.statisticsService.getStatistics(
        parseInt(params["year"])
      );
    });
  }
}
