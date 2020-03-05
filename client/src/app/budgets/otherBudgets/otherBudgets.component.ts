import { Component, OnInit } from '@angular/core';
import { BudgetService } from '../../services/budget.service';
import { Budget } from '../../model/budget';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ActivatedRoute, Params } from '@angular/router';
import { ConfigService } from '../../services/config.service';
import { BudgetType } from '../../model/budgetType';
import { User } from '../../model/user';

@Component({
    selector: 'app-other-budgets',
    templateUrl: './otherBudgets.component.html',
    styleUrls: ['./otherBudgets.component.css']
})
export class OtherBudgetsComponent implements OnInit {
    budgetTypes: BudgetType[];
    budgets: Budget[];
    filteredBudgets: Budget[];
    amount: number;
    year: number;
    currentYear: number;
    selectedYear: number;
    selectedUserId: number;
    budgetTitle: string;

    selectedBudgetType: number;

    availableUsers: User[];

    years: number[];
    rlao: object;
    disableSetOrEditBudgets: boolean;

    private _searchTerm: string;

    get searchTerm(): string {
        return this._searchTerm;
    }

    set searchTerm(value: string) {
        this._searchTerm = value;
        this.filteredBudgets = this.filterBudgets(value);
    }

    filterBudgets(searchString: string) {
        searchString = searchString.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "");

        return this.budgets.filter(budget => budget.user.firstName.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString) !== -1 ||
            budget.user.lastName.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString) !== -1);
    }

    constructor(private budgetService: BudgetService,
        private modalService: NgbModal,
        private route: ActivatedRoute,
        private config: ConfigService) {
        this.years = [];
        this.currentYear = (new Date()).getFullYear();

        for (var year = this.currentYear + 1; year >= config.getOldestYear; year--) {
            this.years.push(year);
        }
    }

    ngOnInit() {

        this.route.params.subscribe((params: Params) => {

            // the following line forces routerLinkActive to update even if the route did nto change
            // see see https://github.com/angular/angular/issues/13865 for futher info
            this.rlao = { dummy: true };

            //var yearParam = this.route.snapshot.paramMap.get('year');
            var yearParam = params['year'];

            this.selectedYear = yearParam != null ? parseInt(yearParam) : this.currentYear;

            this.selectedBudgetType = params['budgetType'];

            if (this.selectedYear == this.currentYear || this.selectedYear == this.currentYear + 1) {
                this.disableSetOrEditBudgets = false;
            }
            else {
                this.disableSetOrEditBudgets = true;
            }

            this.budgetService.getUsersAvailableForBudgetType(this.selectedBudgetType).subscribe(users => this.availableUsers = users);

            this.getActiveUsersBudgets(this.selectedYear);
        });

        this.budgetService.getBudgetsTypes().subscribe(types => {
            this.budgetTypes = types
        });
    }


    getActiveUsersBudgets(year: number): void {
        this.year = year;
        this.budgetService.getCurrentUsersBudgets(year).subscribe(budgets => {
            this.budgets = budgets;
            this.filteredBudgets = budgets.filter(b => {
                return b.type == this.selectedBudgetType;
            })
        });
    }
    setBudgetsForYear(): void {
        if (this.selectedUserId == 0) {
            this.budgetService.createBudgetsForAllActiveUsers(this.budgetTitle, this.amount, this.selectedBudgetType).subscribe(() => this.getActiveUsersBudgets(this.selectedYear));
        } else {
            this.budgetService.createBudget(this.budgetTitle, this.amount, this.selectedUserId, this.selectedBudgetType).subscribe(() => this.getActiveUsersBudgets(this.selectedYear));
        }
    }

    openAmountModal(content) {
        this.modalService.open(content, { centered: true, backdrop: 'static' });
    }
}
