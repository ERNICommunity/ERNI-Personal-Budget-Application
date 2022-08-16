import { Component, OnInit } from '@angular/core';
import { BudgetService } from '../../services/budget.service';
import { Budget } from '../../model/budget';
import { ActivatedRoute, Params } from '@angular/router';
import { ConfigService } from '../../services/config.service';
import { User } from '../../model/user';
import { AlertService } from '../../services/alert.service';
import { MenuItem } from 'primeng/api/menuitem';

@Component({
    selector: 'app-other-budgets',
    templateUrl: './otherBudgets.component.html',
    styleUrls: ['./otherBudgets.component.css']
})
export class OtherBudgetsComponent implements OnInit {
    budgets: Budget[];
    filteredBudgets: Budget[];
    amount: number;
    selectedUserId: number;
    budgetTitle: string;

    budgetTypes: MenuItem[];
    selectedBudgetTypeItem: MenuItem;
    selectedBudgetType: number;

    availableUsers: User[];

    years: MenuItem[];
    selectedYearItem: MenuItem;
    selectedYear: number;

    rlao: object;
    disableSetOrEditBudgets: boolean;
    private _searchTerm: string;
    private _modal: any;

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
        //private modalService: NgbModal,
        private route: ActivatedRoute,
        private alertService: AlertService,
        private config: ConfigService
    ) {
        this.years = [];
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

            this.budgetService
                .getUsersAvailableForBudgetType(this.selectedBudgetType)
                .subscribe((users) => (this.availableUsers = users));

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
    }

    getActiveUsersBudgets(year: number): void {
        this.budgetService.getCurrentUsersBudgets(year).subscribe((budgets) => {
            this.budgets = budgets;
            this.filteredBudgets = budgets.filter((b) => {
                return b.type == this.selectedBudgetType;
            });
        });
    }

    setBudgetsForYear(): void {
        var task =
            this.selectedUserId == 0
                ? this.budgetService.createBudgetsForAllActiveUsers(
                      this.budgetTitle,
                      this.amount,
                      this.selectedBudgetType
                  )
                : this.budgetService.createBudget(
                      this.budgetTitle,
                      this.amount,
                      this.selectedUserId,
                      this.selectedBudgetType
                  );

        task.subscribe(
            () => {
                this.getActiveUsersBudgets(this.selectedYear);
                this.alertService.success('Budget created', 'addOtherBudget');
            },
            (err) => {
                this.alertService.error(
                    'Error while creating budget: ' + JSON.stringify(err.error),
                    'addOtherBudget'
                );
            }
        );
    }

    openAmountModal(content) {
        this.setBudgetToDefault();
        // this._modal = this.modalService.open(content, {
        //   centered: true,
        //   backdrop: "static",
        // });
    }

    close() {
        this._modal.close();
    }

    setBudgetToDefault() {
        this.budgetTitle = '';
        this.amount = 0;
        this.selectedUserId = undefined;
    }
}
