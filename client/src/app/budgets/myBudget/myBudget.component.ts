import {Component, OnInit} from '@angular/core';
import {BudgetService} from '../../services/budget.service';
import {Budget} from '../../model/budget';
import {User} from '../../model/user';
import {Request} from '../../model/request';
import {RequestService} from '../../services/request.service';
import {UserService} from '../../services/user.service';
import {RequestFilter} from '../../requests/requestFilter';


@Component({
    selector: 'app-my-Budget',
    templateUrl: './myBudget.component.html',
    styleUrls: ['./myBudget.component.css']
})
export class MyBudgetComponent implements OnInit {
    budget: Budget;
    requests: Request[];
    requestStateType = RequestFilter;
    user: User;
    year : number;
    currentAmount : number;
    currentYear: number;
    years : number[];

    constructor(private budgetService: BudgetService,
                private requestService: RequestService,
                private userService: UserService)
    {
        this.years = []; 
        this.currentYear = (new Date()).getFullYear();
                
        for (var year = 2017; year <= this.currentYear + 1; year++) {
             this.years.push(year);
        }
    }

    ngOnInit() {
        this.getUser();
        this.getBudget((new Date()).getFullYear());
    }

    getUser(): void {
        this.userService.getCurrentUser()
            .subscribe(user => this.user = user);
    }

    getBudget(year : number) : void {
        this.budgetService.getCurrentUserBudget(year).subscribe(budget => {
            this.budget = budget,
            this.getRequests(year)
        });
    }

    getRequests(year: number): void {
        this.year = year;
        this.requestService.getRequests(year)
            .subscribe(requests => {this.requests = requests, this.getCurrentAmount(requests)});
    }

    getCurrentAmount(requests : Request[]) : void {
        var requestsSum = 0;

        requests.forEach((req) =>{
            requestsSum = requestsSum + req.amount});
        
        this.currentAmount = this.budget.amount - requestsSum;
    }

    deleteRequest(id: number): void {
        this.requestService.deleteRequest(id).subscribe(() => this.requests = this.requests.filter(req => req.id !== id));
      }
}
