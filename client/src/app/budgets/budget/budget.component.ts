import { Component, OnInit, Input } from '@angular/core';
import { Budget } from '../../model/budget';

@Component({
  selector: 'app-budget',
  templateUrl: './budget.component.html',
  styleUrls: ['./budget.component.css']
})
export class BudgetComponent implements OnInit {

  @Input() budget: Budget;
  
  constructor() { }

  ngOnInit() {
  }

}
