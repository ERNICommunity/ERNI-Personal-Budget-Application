import { Component, inject, signal } from "@angular/core";
import { BudgetService } from "../services/budget.service";
import { ActivatedRoute } from "@angular/router";
import { combineLatest } from "rxjs";
import { DataChangeNotificationService } from "../services/dataChangeNotification.service";
import { map, switchMap, tap } from "rxjs/operators";
import { MenuHelper } from "../shared/menu-helper";
import { toSignal } from "@angular/core/rxjs-interop";
import { SharedModule } from "../shared/shared.module";
import { BudgetComponent } from "./budget/budget.component";

@Component({
  selector: "app-my-budget",
  templateUrl: "./myBudget.component.html",
  styleUrls: ["./myBudget.component.css"],
  standalone: true,
  imports: [BudgetComponent, SharedModule],
})
export class MyBudgetComponent {
  #budgetService = inject(BudgetService);
  #route = inject(ActivatedRoute);
  #dataChangeNotificationService = inject(DataChangeNotificationService);

  isBusy = signal(false);

  #budgetTypes = this.#budgetService.getBudgetsTypes();

  #rawBudgets = combineLatest([
    this.#route.params,
    this.#dataChangeNotificationService.notifications$,
  ]).pipe(
    tap(() => this.isBusy.set(true)),
    map(([params]) => +params["year"]),
    switchMap((_) => this.#budgetService.getCurrentUserBudgets(_)),
    tap((_) => this.isBusy.set(false))
  );

  budgets = toSignal(
    combineLatest([this.#rawBudgets, this.#budgetTypes]).pipe(
      map(([budgets, budgetTypes]) =>
        budgets.map((budget) => ({
          ...budget,
          typeName: budgetTypes.find((_) => _.id === budget.type)?.name ?? "",
        }))
      )
    ),
    { initialValue: [] }
  );

  years = MenuHelper.getYearMenu((year) => ["/my-budget", year]);
}
