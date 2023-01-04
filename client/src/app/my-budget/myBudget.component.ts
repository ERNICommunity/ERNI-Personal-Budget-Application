import { Component, OnInit } from '@angular/core';
import { BudgetService } from '../services/budget.service';
import { Budget } from '../model/budget';
import { ActivatedRoute } from '@angular/router';
import { BusyIndicatorService } from '../services/busy-indicator.service';
import { combineLatest } from 'rxjs';
import { DataChangeNotificationService } from '../services/dataChangeNotification.service';
import { MenuItem } from 'primeng/api/menuitem';
import { map, switchMap, tap } from 'rxjs/operators';
import { MenuHelper } from '../shared/menu-helper';

@Component({
    selector: 'app-my-Budget',
    templateUrl: './myBudget.component.html',
    styleUrls: ['./myBudget.component.css']
})
export class MyBudgetComponent implements OnInit {
    budgets: Budget[];

    years: MenuItem[];

    currentYear: number;

    isBusy: boolean;

    constructor(
        private budgetService: BudgetService,
        private route: ActivatedRoute,
        private dataChangeNotificationService: DataChangeNotificationService
    ) {
        this.years = MenuHelper.getYearMenu((year) => ['/my-budget', year]);
    }

    ngOnInit() {
        combineLatest([
            this.route.params,
            this.dataChangeNotificationService.notifications$
        ])
            .pipe(
                tap((_) => (this.budgets = [])),
                map(([params]) => +params['year']),
                switchMap((_) => this.budgetService.getCurrentUserBudgets(_))
            )
            .subscribe((_) => {
                this.budgets = _;
                this.isBusy = false;
            });
    }
}
