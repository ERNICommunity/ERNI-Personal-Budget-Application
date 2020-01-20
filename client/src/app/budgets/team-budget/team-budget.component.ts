import { Component, OnInit } from '@angular/core';
import { BudgetService } from '../../services/budget.service';
import { TeamBudget } from '../../model/request/team-budget';

@Component({
    selector: 'app-team-budget',
    templateUrl: './team-budget.component.html',
    styleUrls: ['./team-budget.component.css']
})
export class TeamBudgetComponent implements OnInit {
    budget: TeamBudget;
    currentYear: number;

    constructor(private budgetService: BudgetService) { }

    ngOnInit() {
        this.currentYear = (new Date()).getFullYear();

        this.budgetService.getTeamBudgets(this.currentYear).subscribe(budget => this.budget = budget);
    }
}
