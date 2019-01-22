import { Component, OnInit } from '@angular/core';
import { BudgetService } from '../../services/budget.service';
import { Budget } from '../../model/budget';
import { User } from '../../model/user';
import { UserState } from '../../model/userState';
import { Request } from '../../model/request';
import { RequestService } from '../../services/request.service';
import { UserService } from '../../services/user.service';
import { RequestFilter } from '../../requests/requestFilter';
import { ActivatedRoute, Params } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfigService } from '../../services/config.service';
import { BusyIndicatorService } from '../../services/busy-indicator.service';

@Component({
    selector: 'app-my-Budget',
    templateUrl: './myBudget.component.html',
    styleUrls: ['./myBudget.component.css']
})
export class MyBudgetComponent implements OnInit {
    budget: Budget;
    requests: Request[];
    requestStateType = RequestFilter;
    userState = UserState;
    user: User;
    currentAmount: number;
    currentYear: number;
    selectedYear: number;
    years: number[];
    rlao: object;

    constructor(private budgetService: BudgetService,
        private requestService: RequestService,
        private userService: UserService,
        private modalService: NgbModal,
        private route: ActivatedRoute,
        private config: ConfigService,
        public busyIndicatorService: BusyIndicatorService) {
        this.years = [];
        this.currentYear = (new Date()).getFullYear();

        for (var year = this.currentYear; year >= config.getOldestYear; year--) {
            this.years.push(year);
        }
    }

    ngOnInit() {

        this.getUser();

        this.route.params.subscribe((params: Params) => {

            // the following line forces routerLinkActive to update even if the route did nto change
            // see see https://github.com/angular/angular/issues/13865 for futher info
            this.rlao = { dummy: true };

            //var yearParam = this.route.snapshot.paramMap.get('year');
            var yearParam = params['year'];

            this.selectedYear = yearParam != null ? parseInt(yearParam) : this.currentYear;

            this.getBudget(this.selectedYear);
        });
    }

    getUser(): void {
        this.userService.getCurrentUser()
            .subscribe(user => this.user = user);
    }

    getBudget(year: number): void {
        this.busyIndicatorService.start();
        this.budgetService.getCurrentUserBudget(year).subscribe(budget => {
            this.budget = budget
            if (budget) {
                this.getRequests(year)
            }
            this.busyIndicatorService.end();
        });
    }

    getRequests(year: number): void {
        this.busyIndicatorService.start();
        this.requestService.getRequests(year)
            .subscribe(requests => { 
                this.requests = requests, this.getCurrentAmount(requests) 
                this.busyIndicatorService.end();
            });
    }

    getCurrentAmount(requests: Request[]): void {
        var requestsSum = 0;

        requests.forEach((req) => {
            requestsSum += req.amount
        });

        this.currentAmount = this.budget.amount - requestsSum;
    }

    openDeleteConfirmationModal(content) {
        this.modalService.open(content, { centered: true, backdrop: 'static' });
    }

    deleteRequest(id: number): void {
        this.requestService.deleteRequest(id).subscribe(() => {
            this.requests = this.requests.filter(req => req.id !== id),
                this.getCurrentAmount(this.requests)
        });
    }
}
