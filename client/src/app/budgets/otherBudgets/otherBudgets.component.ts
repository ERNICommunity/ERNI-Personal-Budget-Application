import { Component, OnInit } from "@angular/core";
import { BudgetService } from "../../services/budget.service";
import { Budget } from "../../model/budget";
import { ActivatedRoute } from "@angular/router";
import { MenuItem } from "primeng/api/menuitem";
import { DataChangeNotificationService } from "../../services/dataChangeNotification.service";
import { BehaviorSubject, combineLatest, Observable } from "rxjs";
import { distinctUntilChanged, map, switchMap } from "rxjs/operators";
import { MenuHelper } from "../../shared/menu-helper";

@Component({
  selector: "app-other-budgets",
  templateUrl: "./otherBudgets.component.html",
  styleUrls: ["./otherBudgets.component.css"],
})
export class OtherBudgetsComponent implements OnInit {
  years$: Observable<MenuItem[]>;
  budgetTypes$: Observable<MenuItem[]>;

  budgets$: Observable<Budget[]>;

  disableSetOrEditBudgets$: Observable<boolean>;

  selectedYear$: Observable<number>;
  selectedBudgetType$: Observable<number>;

  private searchTerm$ = new BehaviorSubject<string | null>(null);

  get searchTerm(): string | null {
    return this.searchTerm$.value;
  }

  set searchTerm(value: string) {
    this.searchTerm$.next(value);
  }

  constructor(
    private budgetService: BudgetService,
    private route: ActivatedRoute,
    private notificationService: DataChangeNotificationService
  ) {}

  ngOnInit() {
    this.selectedYear$ = this.route.params.pipe(
      map((params) => +params["year"]),
      distinctUntilChanged()
    );

    this.selectedBudgetType$ = this.route.params.pipe(
      map((params) => +params["budgetType"]),
      distinctUntilChanged()
    );

    const currentYear = new Date().getFullYear();

    this.disableSetOrEditBudgets$ = this.selectedYear$.pipe(
      map(
        (selectedYear) =>
          selectedYear === currentYear || selectedYear === currentYear + 1
      )
    );

    this.years$ = this.selectedBudgetType$.pipe(
      map((budgetType) =>
        MenuHelper.getYearMenu((year) => ["/budgets/", year, budgetType])
      )
    );

    const budgets$ = combineLatest([
      this.selectedYear$,
      this.notificationService.notifications$,
    ]).pipe(
      switchMap(([year]) => this.budgetService.getActiveUsersBudgets(year))
    );

    this.budgets$ = combineLatest([
      budgets$,
      this.searchTerm$,
      this.selectedBudgetType$,
    ]).pipe(
      map(([budgets, searchTerm, budgetType]) =>
        this.filterBudgets(budgets, searchTerm ?? "", budgetType)
      )
    );

    this.budgetTypes$ = combineLatest([
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
  }

  filterBudgets(budgets: Budget[], searchString: string, budgetType: number) {
    searchString = searchString ?? "";

    searchString = searchString
      .toLowerCase()
      .normalize("NFD")
      .replace(/[\u0300-\u036f]/g, "");

    return budgets
      .filter((b) => b.type == budgetType)
      .filter(
        (budget) =>
          budget.user.firstName
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "")
            .indexOf(searchString) !== -1 ||
          budget.user.lastName
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "")
            .indexOf(searchString) !== -1
      );
  }
}
