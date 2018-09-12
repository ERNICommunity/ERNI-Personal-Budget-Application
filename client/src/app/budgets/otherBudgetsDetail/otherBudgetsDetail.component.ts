import {Component, OnInit} from '@angular/core';
import { Location } from '@angular/common';
import {Budget} from '../../model/budget';
import {BudgetService} from '../../services/budget.service';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-other-budgets-detail',
    templateUrl: 'otherBudgetsDetail.component.html',
    styleUrls: ['otherBudgetsDetail.component.css']
})
export class OtherBudgetsDetailComponent implements OnInit {
    budget: Budget;


    constructor(private budgetService: BudgetService,
                private location: Location,
                private route: ActivatedRoute) {
    }

    ngOnInit() {
        this.getCurrentUserBudget();
    }

    getCurrentUserBudget(): void {
        const id = this.route.snapshot.paramMap.get('id');
        const year = this.route.snapshot.paramMap.get('year');

        this.budgetService.getCurrentUserBudgetByYear(id,parseInt(year))
            .subscribe(budget => this.budget = budget);
    }

    goBack(): void {
        this.location.back();
      }
    
    save() : void {
        this.budgetService.updateBudget(this.budget)
            .subscribe(() => this.goBack())
    }

}
