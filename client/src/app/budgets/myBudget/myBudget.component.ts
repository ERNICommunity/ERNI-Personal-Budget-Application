import {Component, OnInit} from '@angular/core';
import {BudgetService} from '../../services/budget.service';
import {Budget} from '../../model/budget';
import {User} from '../../model/user';
import {Request} from '../../model/request';
import {RequestService} from '../../services/request.service';
import {UserService} from '../../services/user.service';
import {RequestFilter} from '../../requests/requestFilter';
import { ActivatedRoute, Params } from '@angular/router';

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
    selectedYear: number;
    years : number[];
    rlao: object;

    constructor(private budgetService: BudgetService,
                private requestService: RequestService,
                private userService: UserService,
                private route: ActivatedRoute)
    {
        this.years = []; 
        this.currentYear = (new Date()).getFullYear();
                
        for (var year = 2017; year <= this.currentYear + 1; year++) {
             this.years.push(year);
        }
    }

    ngOnInit() {

        this.getUser();

        this.route.params.subscribe((params: Params) => {

            // the following line forces routerLinkActive to update even if the route did nto change
            // see see https://github.com/angular/angular/issues/13865 for futher info
            this.rlao = {dummy: true};

            //var yearParam = this.route.snapshot.paramMap.get('year');
            var yearParam = params['year']; 
            console.log('yearParam=' + yearParam);

            this.selectedYear = yearParam != null ? parseInt(yearParam) : this.currentYear;
            //console.log(this.selectedYear);
            this.getBudget(this.selectedYear);
          });
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
