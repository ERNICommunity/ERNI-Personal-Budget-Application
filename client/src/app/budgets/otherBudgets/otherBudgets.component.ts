import { Component, computed } from "@angular/core";
import { BudgetService } from "../../services/budget.service";
import { Budget } from "../../model/budget";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { toObservable, toSignal } from "@angular/core/rxjs-interop";
import { DataChangeNotificationService } from "../../services/dataChangeNotification.service";
import { BehaviorSubject, combineLatest } from "rxjs";
import { distinctUntilChanged, map, switchMap } from "rxjs/operators";
import { MenuHelper } from "../../shared/menu-helper";
import { AsyncPipe } from "@angular/common";
import { AuthDirective } from "../../shared/directives/authDirective";
import { SharedModule } from "primeng/api";
import { TableModule } from "primeng/table";
import { InputTextModule } from "primeng/inputtext";
import { FormsModule } from "@angular/forms";
import { CreateBudgetsComponent } from "../create-budgets/create-budgets.component";
import { AccordionModule } from "primeng/accordion";
import { TabMenuModule } from "primeng/tabmenu";

@Component({
  selector: "app-other-budgets",
  templateUrl: "./otherBudgets.component.html",
  styleUrls: ["./otherBudgets.component.css"],
  standalone: true,
  imports: [
    TabMenuModule,
    AccordionModule,
    CreateBudgetsComponent,
    FormsModule,
    InputTextModule,
    TableModule,
    SharedModule,
    AuthDirective,
    RouterLink,
    AsyncPipe,
  ],
})
export class OtherBudgetsComponent {
  currentYear = new Date().getFullYear();

  selectedYear = toSignal(
    this.route.params.pipe(
      map((params) => +params["year"]),
      distinctUntilChanged()
    ),
    { initialValue: this.currentYear }
  );

  budgetTypes$ = combineLatest([
    this.budgetService.getBudgetsTypes(),
    toObservable(this.selectedYear),
  ]).pipe(
    map(([types, year]) =>
      types.map((type) => ({
        label: type.name,
        routerLink: ["/budgets/", year, type.id],
      }))
    )
  );

  selectedBudgetType$ = this.route.params.pipe(
    map((params) => +params["budgetType"]),
    distinctUntilChanged()
  );

  disableSetOrEditBudgets = computed(
    () =>
      this.selectedYear() === this.currentYear ||
      this.selectedYear() === this.currentYear + 1
  );

  years$ = this.selectedBudgetType$.pipe(
    map((budgetType) =>
      MenuHelper.getYearMenu((year) => ["/budgets/", year, budgetType])
    )
  );

  private searchTerm$: BehaviorSubject<string> = new BehaviorSubject("");

  #unfilteredbudgets$ = combineLatest([
    toObservable(this.selectedYear),
    this.notificationService.notifications$,
  ]).pipe(
    switchMap(([year, _]) => this.budgetService.getActiveUsersBudgets(year))
  );

  budgets$ = combineLatest([
    this.#unfilteredbudgets$,
    this.searchTerm$,
    this.selectedBudgetType$,
  ]).pipe(
    map(([budgets, searchTerm, budgetType]) =>
      this.filterBudgets(budgets, searchTerm, budgetType)
    )
  );

  get searchTerm(): string {
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
