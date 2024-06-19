import { Component, inject } from "@angular/core";
import { BudgetService } from "../services/budget.service";
import { ActivatedRoute } from "@angular/router";
import { combineLatest } from "rxjs";
import { DataChangeNotificationService } from "../services/dataChangeNotification.service";
import { map, switchMap, tap } from "rxjs/operators";
import { MenuHelper } from "../shared/menu-helper";
import { toSignal } from "@angular/core/rxjs-interop";

@Component({
  selector: "app-my-budget",
  templateUrl: "./myBudget.component.html",
  styleUrls: ["./myBudget.component.css"],
})
export class MyBudgetComponent {
  #budgetService = inject(BudgetService);
  #route = inject(ActivatedRoute);
  #dataChangeNotificationService = inject(DataChangeNotificationService);

  budgets = toSignal(
    combineLatest([
      this.#route.params,
      this.#dataChangeNotificationService.notifications$,
    ]).pipe(
      tap((_) => (this.isBusy = true)),
      map(([params]) => +params["year"]),
      switchMap((_) => this.#budgetService.getCurrentUserBudgets(_)),
      tap((_) => (this.isBusy = false))
    )
  );

  currentYear: number;

  isBusy: boolean;

  years = MenuHelper.getYearMenu((year) => ["/my-budget", year]);
}
