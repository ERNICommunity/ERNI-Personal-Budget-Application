import { Component, OnInit } from '@angular/core';
import { Request } from '../model/request';

@Component({
  selector: 'app-budgets',
  templateUrl: './budgets.component.html',
  styleUrls: ['./budgets.component.css']
})
export class BudgetsComponent implements OnInit {
  requests: Request[];

  constructor() { }

  ngOnInit() {
  }
}
