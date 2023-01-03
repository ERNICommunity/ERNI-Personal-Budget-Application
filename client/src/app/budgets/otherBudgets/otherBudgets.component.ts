import { Component, OnDestroy, OnInit } from '@angular/core';
import { BudgetService } from '../../services/budget.service';
import { Budget } from '../../model/budget';
import { ActivatedRoute, Params } from '@angular/router';
import { ConfigService } from '../../services/config.service';
import { MenuItem } from 'primeng/api/menuitem';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    selector: 'app-other-budgets',
    templateUrl: './otherBudgets.component.html',
    styleUrls: ['./otherBudgets.component.css']
})
export class OtherBudgetsComponent implements OnInit, OnDestroy {
    private destroy$ = new Subject<void>();

    budgets: Budget[];
    filteredBudgets: Budget[];
    selectedUserId: number;

    budgetTypes: MenuItem[];
    selectedBudgetTypeItem: MenuItem;
    selectedBudgetType: number;

    years: MenuItem[];
    selectedYearItem: MenuItem;
    selectedYear: number;

    disableSetOrEditBudgets: boolean;
    public _searchTerm: string;

    get searchTerm(): string {
        return this._searchTerm;
    }

    set searchTerm(value: string) {
        this._searchTerm = value;
        this.filteredBudgets = this.filterBudgets(value);
    }

    filterBudgets(searchString: string) {
        searchString = searchString
            .toLowerCase()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '');

        return this.budgets
            .filter((b) => b.type == this.selectedBudgetType)
            .filter(
                (budget) =>
                    budget.user.firstName
                        .toLowerCase()
                        .normalize('NFD')
                        .replace(/[\u0300-\u036f]/g, '')
                        .indexOf(searchString) !== -1 ||
                    budget.user.lastName
                        .toLowerCase()
                        .normalize('NFD')
                        .replace(/[\u0300-\u036f]/g, '')
                        .indexOf(searchString) !== -1
            );
    }

    constructor(
        private budgetService: BudgetService,
        private route: ActivatedRoute,
        private config: ConfigService,
        private notificationService: DataChangeNotificationService
    ) {
        this.years = [];
    }
    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    ngOnInit() {
        this.route.params.subscribe((params: Params) => {
            var currentYear = new Date().getFullYear();

            this.selectedYear =
                params['year'] != null ? parseInt(params['year']) : currentYear;

            this.selectedBudgetType = Number(params['budgetType']);

            this.years = [];
            for (
                var year = new Date().getFullYear() + 1;
                year >= this.config.getOldestYear;
                year--
            ) {
                this.years.push({
                    label: year.toString(),
                    routerLink: [
                        '/other-budgets/',
                        year,
                        this.selectedBudgetType
                    ]
                });
            }

            this.selectedYearItem = this.years.find(
                (_) => _.label == this.selectedYear.toString()
            );

            if (
                this.selectedYear == currentYear ||
                this.selectedYear == currentYear + 1
            ) {
                this.disableSetOrEditBudgets = false;
            } else {
                this.disableSetOrEditBudgets = true;
            }

            this.getActiveUsersBudgets(this.selectedYear);
        });

        this.budgetService.getBudgetsTypes().subscribe((types) => {
            this.budgetTypes = [];
            var dict = [];

            types.forEach((type) => {
                var item = {
                    label: type.name,
                    routerLink: [
                        '/other-budgets/',
                        this.selectedYear.toString(),
                        type.id
                    ]
                };
                this.budgetTypes.push(item);

                dict[type.id] = item;
            });

            this.selectedBudgetTypeItem = dict[this.selectedBudgetType];
        });

        this.notificationService.notifications$
            .pipe(takeUntil(this.destroy$))
            .subscribe((_) => {
                this.getActiveUsersBudgets(this.selectedYear);
            });
    }

    getActiveUsersBudgets(year: number): void {
        this.budgetService.getCurrentUsersBudgets(year).subscribe((budgets) => {
            this.budgets = budgets;
            this.filteredBudgets = budgets.filter((b) => {
                return b.type == this.selectedBudgetType;
            });
        });
    }
}
