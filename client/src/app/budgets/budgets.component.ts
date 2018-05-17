import { Component, OnInit } from '@angular/core';
import { BudgetService } from '../services/budget.service';
import { Budget } from '../model/budget';
import { Request } from '../model/request';
import { RequestService } from '../services/request.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-users',
  templateUrl: './budgets.component.html',
  styleUrls: ['./budgets.component.css']
})
export class BudgetsComponent implements OnInit {
  budgets: Budget[];
  requests: Request[];

  constructor(private budgetService: BudgetService, private requestService: RequestService, private route: ActivatedRoute,) { }

  ngOnInit() {
    this.getBudgets();

    const year = this.route.snapshot.paramMap.get('year');

    this.getRequests(Number(year));
  }

  getBudgets(): void {
    this.budgetService.getCurrentUsersBudgets()
      .subscribe(budgets => this.budgets = budgets);
  }

  getRequests(year: number): void {
    this.requestService.getRequests(2018)
      .subscribe(requests => this.requests = requests);
  }


}
