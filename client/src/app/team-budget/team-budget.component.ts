import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { MenuItem } from "primeng/api/menuitem";
import { combineLatest } from "rxjs";
import { DataChangeNotificationService } from "../services/dataChangeNotification.service";

@Component({
  selector: "app-team-budget",
  templateUrl: "./team-budget.component.html",
  styleUrls: ["./team-budget.component.css"],
})
export class TeamBudgetComponent implements OnInit {
  years: MenuItem[];
  selectedYear: MenuItem;

  currentYear: number;

  rlao: object;

  constructor(
    private route: ActivatedRoute,
    private dataChangeNotificationService: DataChangeNotificationService
  ) {
    this.years = [];

    this.currentYear = new Date().getFullYear();

    for (var year = this.currentYear; year >= 2015; year--) {
      this.years.push({
        label: year.toString(),
        routerLink: ["/team-budget", year],
      });
    }
  }

  ngOnInit() {
    combineLatest(
      this.route.params,
      this.dataChangeNotificationService.notifications$
    ).subscribe(([params]) => {
      // the following line forces routerLinkActive to update even if the route did nto change
      // see see https://github.com/angular/angular/issues/13865 for futher info
      this.rlao = { dummy: true };

      //var yearParam = this.route.snapshot.paramMap.get('year');
      var yearParam = params["year"];

      this.selectedYear = this.years.find(_ => _.label == (yearParam != null ? parseInt(yearParam) : this.currentYear).toString());
        
    });
  }
}
