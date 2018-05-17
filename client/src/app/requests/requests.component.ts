import { Component, OnInit } from '@angular/core';
import { Request } from '../model/request';
import { RequestService } from '../services/request.service';

@Component({
  selector: 'app-requests',
  templateUrl: './requests.component.html',
  styleUrls: ['./requests.component.css']
})
export class RequestsComponent implements OnInit {
  requests: Request[];

  constructor(private valueService: RequestService) { }

  ngOnInit() {
    this.getHeroes();
  }

  getHeroes(): void {
    this.valueService.getRequests(2018)
      .subscribe(heroes => this.requests = heroes);
  }

}
