import { Component, computed, inject, model } from "@angular/core";
import { BudgetService } from "../../services/budget.service";
import { Budget } from "../../model/budget";
import { ActivatedRoute } from "@angular/router";
import { DataChangeNotificationService } from "../../services/dataChangeNotification.service";
import { combineLatest } from "rxjs";
import { distinctUntilChanged, map, switchMap } from "rxjs/operators";
import { MenuHelper } from "../../shared/menu-helper";
import { toSignal } from "@angular/core/rxjs-interop";
import {normalize} from "../../utils/normalizer.util";

@Component({
  selector: "app-other-budgets",
  templateUrl: "./otherBudgets.component.html",
  styleUrls: ["./otherBudgets.component.css"],
})
export class OtherBudgetsComponent {
  budgetService = inject(BudgetService);
  route = inject(ActivatedRoute);
  notificationService = inject(DataChangeNotificationService);

  selectedYear$ = this.route.params.pipe(
    map((params) => +params["year"]),
    distinctUntilChanged()
  );

  selectedBudgetType = toSignal(
    this.route.params.pipe(
      map((params) => +params["budgetType"]),
      distinctUntilChanged()
    ),
    { initialValue: undefined }
  );

  years = computed(() => {
    const selectedBudgetType = this.selectedBudgetType();
    return selectedBudgetType
      ? MenuHelper.getYearMenu((year) => [
          "/budgets/",
          year,
          selectedBudgetType,
        ])
      : [];
  });

  budgetTypes$ = combineLatest([
    this.budgetService.getBudgetsTypes(),
    this.selectedYear$,
  ]).pipe(
    map(([types, year]) =>
      types.map((type) => ({
        label: type.name,
        routerLink: ["/budgets/", year, type.id],
      }))
    )
  );

  #budgets = toSignal(
    combineLatest([
      this.selectedYear$,
      this.notificationService.notifications$,
    ]).pipe(
      switchMap(([year]) => this.budgetService.getActiveUsersBudgets(year))
    ),
    { initialValue: [] }
  );

  budgets = computed(() => {
    const selectedBudgetType = this.selectedBudgetType();

    if (!selectedBudgetType) {
      return [];
    }

    return this.filterBudgets(
      this.#budgets(),
      this.searchTerm(),
      selectedBudgetType
    );
  });

  searchTerm = model("");

  filterBudgets(budgets: Budget[], searchString: string, budgetType: number) {
    searchString = normalize(searchString ?? "");

    return budgets
      .filter((b) => b.type == budgetType)
      .filter(
        (budget) =>
          normalize(budget.user.firstName).indexOf(searchString) !== -1 ||
          normalize(budget.user.lastName).indexOf(searchString) !== -1
      );
  }
}
