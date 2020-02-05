import { Component, OnInit, Input } from '@angular/core';
import { BudgetService } from '../../services/budget.service';
import { TeamBudget } from '../../model/request/team-budget';
import { RequestFilter } from '../../requests/requestFilter';
import { Router, ActivatedRoute, Params } from '@angular/router';

@Component({
    selector: 'app-team-budget',
    templateUrl: './team-budget.component.html',
    styleUrls: ['./team-budget.component.css']
})
export class TeamBudgetComponent implements OnInit {
    budget: TeamBudget;
    requestStateType = RequestFilter;

    @Input() year: number;

    constructor(private route: ActivatedRoute, private budgetService: BudgetService) { }

    ngOnInit() {
        this.route.params.subscribe((params: Params) => {
            var yearParam = params['year'];
            this.budgetService.getTeamBudgets(yearParam).subscribe(budget => {
                this.budget = budget;
            });
        });
    }
}
