import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { combineLatest } from 'rxjs';
import { DataChangeNotificationService } from '../services/dataChangeNotification.service';

@Component({
  selector: 'app-team-budget',
  templateUrl: './team-budget.component.html',
  styleUrls: ['./team-budget.component.css']
})
export class TeamBudgetComponent implements OnInit {

  years: number[];
  currentYear: number;
  selectedYear: number;

  rlao: object;

  constructor(private route: ActivatedRoute,
    private dataChangeNotificationService: DataChangeNotificationService
    ) {
    this.years = [];
        this.currentYear = (new Date()).getFullYear();

        for (var year = this.currentYear; year >= 2020; year--) {
            this.years.push(year);
        }
  }

  ngOnInit() {
    combineLatest(this.route.params, this.dataChangeNotificationService.notifications$).subscribe(([params]) => {
        // the following line forces routerLinkActive to update even if the route did nto change
        // see see https://github.com/angular/angular/issues/13865 for futher info
        this.rlao = { dummy: true };

        //var yearParam = this.route.snapshot.paramMap.get('year');
        var yearParam = params['year'];

        this.selectedYear = yearParam != null ? parseInt(yearParam) : this.currentYear;
    });
}

}
