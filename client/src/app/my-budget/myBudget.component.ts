import { Component, OnInit } from "@angular/core";
import { BudgetService } from "../services/budget.service";
import { ActivatedRoute } from "@angular/router";
import { combineLatest } from "rxjs";
import { DataChangeNotificationService } from "../services/dataChangeNotification.service";
import { MenuItem } from "primeng/api/menuitem";
import { map, switchMap, tap } from "rxjs/operators";
import { MenuHelper } from "../shared/menu-helper";
import { CurrentUsersBudget } from "../model/current-users-budget";

@Component({
  selector: "app-my-Budget",
  templateUrl: "./myBudget.component.html",
  styleUrls: ["./myBudget.component.css"],
})
export class MyBudgetComponent {
  budgets = combineLatest([
    this.route.params,
    this.dataChangeNotificationService.notifications$,
  ]).pipe(
    tap(() => (this.isBusy = true)),
    map(([params]) => +params["year"]),
    switchMap((_) => this.budgetService.getCurrentUserBudgets(_)),
    tap(() => (this.isBusy = false))
  );

  years: MenuItem[];

  isBusy = false;

  constructor(
    private budgetService: BudgetService,
    private route: ActivatedRoute,
    private dataChangeNotificationService: DataChangeNotificationService
  ) {
    this.years = MenuHelper.getYearMenu((year) => ["/my-budget", year]);
  }
}
