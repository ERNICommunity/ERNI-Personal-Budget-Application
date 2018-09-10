import { Component, OnInit } from '@angular/core';
import { BudgetService } from '../../services/budget.service';
import { Budget } from '../../model/budget';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import { ActivatedRoute, Params } from '@angular/router';

@Component({
  selector: 'app-other-budgets',
  templateUrl: './otherBudgets.component.html',
  styleUrls: ['./otherBudgets.component.css']
})
export class OtherBudgetsComponent implements OnInit {
  budgets : Budget[];
  amount : number;
  year : number;
  currentYear: number;
  years : number[];

  
  constructor(private budgetService: BudgetService,private modalService: NgbModal,private route: ActivatedRoute) {
      this.years = []; 
        this.currentYear = (new Date()).getFullYear();
                
        for (var year = 2017; year <= this.currentYear + 1; year++) {
             this.years.push(year);
        }
   }

  ngOnInit() {
    this.getActiveUsersBudgets((new Date()).getFullYear());
  }

  getActiveUsersBudgets(year : number): void {
    this.year = year;
    this.budgetService.getCurrentUsersBudgets(year).subscribe(budgets => this.budgets = budgets);
  }

  setBudgetsForCurrentYear() :void {
    console.log(this.amount);
    this.budgetService.setBudgetsForCurrentYear(this.amount).subscribe(() => this.getActiveUsersBudgets((new Date()).getFullYear()));
  }

  openAmountModal(content) {
    this.modalService.open(content, { centered : true  });
  }

}
