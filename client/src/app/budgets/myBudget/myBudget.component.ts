import { Component, OnInit } from '@angular/core';
import { BudgetService } from '../../services/budget.service';
import { Budget } from '../../model/budget';
import { User } from '../../model/user';
import { UserState } from '../../model/userState';
import { UserService } from '../../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { ConfigService } from '../../services/config.service';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { combineLatest, forkJoin, of, Observable } from 'rxjs';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { TeamBudgetService } from '../../services/team-budget.service';
import { catchError, map } from 'rxjs/operators';

@Component({
    selector: 'app-my-Budget',
    templateUrl: './myBudget.component.html',
    styleUrls: ['./myBudget.component.css']
})
export class MyBudgetComponent implements OnInit {
    budgets: Budget[];
    userState = UserState;
    currentYear: number;
    selectedYear: number;
    years: number[];
    rlao: object;

    constructor(
        private budgetService: BudgetService,
        private teamBudgetService: TeamBudgetService,
        private route: ActivatedRoute,
        config: ConfigService,
        public busyIndicatorService: BusyIndicatorService,
        private dataChangeNotificationService: DataChangeNotificationService) {
        this.years = [];
        this.currentYear = (new Date()).getFullYear();

        for (var year = this.currentYear; year >= config.getOldestYear; year--) {
            this.years.push(year);
        }
    }

    ngOnInit() {
        combineLatest(this.route.params, this.dataChangeNotificationService.notifications$).subscribe(([params]) => {
            // the following line forces routerLinkActive to update even if the route did nto change
            // see see https://github.com/angular/angular/issues/13865 for futher info
            this.rlao = { dummy: true };

            //var yearParam = this.route.snapshot.paramMap.get('year');
            var yearParam = params['year'];

            this.selectedYear = yearParam != null ? parseInt(yearParam) : this.currentYear;

            this.getBudgets(this.selectedYear);
        });
    }

    getBudgets(year: number): void {
        this.budgets = [];
        this.busyIndicatorService.start();

        let requests = [this.budgetService.getCurrentUserBudgets(year)
            .pipe(map(this.handleResponse), catchError(this.handleError))];

        requests.push(this.teamBudgetService.getCurrentUserBudgets(year)
            .pipe(map(this.handleResponse), catchError(this.handleError)));

        forkJoin(requests).subscribe(
            data => {
                data.forEach(budgets => {
                    if (budgets.length > 0) {
                        this.budgets = this.budgets.concat(budgets)
                    }
                });
            })
            .add(() => this.busyIndicatorService.end());
    }

    private handleResponse(data) {
        if (data == null)
            return of([]);

        return data;
    }

    private handleError() {
        return of([]);
    }
}
