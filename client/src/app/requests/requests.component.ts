import { Component, OnInit } from '@angular/core';
import { Request } from '../model/request/request';
import { RequestService } from '../services/request.service';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-requests',
    templateUrl: './requests.component.html',
    styleUrls: ['./requests.component.css'],
    standalone: true,
    imports: [RouterOutlet]
})
export class RequestsComponent implements OnInit {
  requests: Request[];

  constructor(private valueService: RequestService) { }

  ngOnInit() {
    // this.getPendingRequests();
  }

  // getPendingRequests(): void {
  //   this.valueService.getPendingRequests()
  //     .subscribe(requests => this.requests = requests);
  // }

}
