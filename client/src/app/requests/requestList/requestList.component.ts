import {Component, OnInit} from '@angular/core';
import {Request} from '../../model/request';
import {RequestService} from '../../services/request.service';
import { RequestFilter } from '../requestFilter';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';

@Component({
    selector: 'app-users',
    templateUrl: 'requestList.component.html',
    styleUrls: ['requestList.component.css']
})
export class RequestListComponent implements OnInit {
    requests: Request[];
    requestFilter : RequestFilter;

    constructor(private requestService: RequestService, private route: ActivatedRoute) {
    }

    ngOnInit() {
        var filter = <RequestFilter>this.route.snapshot.data['filter'];

        var year = <number>this.route.snapshot.paramMap['year'];

        this.getRequests(filter, 2018); // year);
    }

    getRequests(filter: RequestFilter, year: number): void {
        var requests: Observable<Request[]>;

        switch (filter) {
            case RequestFilter.Approved:
                requests = this.requestService.getApprovedRequests(year);
                this.requestFilter = filter;
                break;
            case RequestFilter.Pending:
                requests = this.requestService.getPendingRequests(year);
                this.requestFilter = filter;
                break;
            case RequestFilter.Rejected:
                requests = this.requestService.getRejectedRequests(year);
                this.requestFilter = filter;
                break;
            default:
                break;
        }

        requests.subscribe(requests => this.requests = requests);
    }

    approveRequest(id: number): void {
        this.requests = this.requests.filter(req => req.id !== id);
        this.requestService.approveRequest(id).subscribe();
      }

    rejectRequest(id: number): void {
        this.requests = this.requests.filter(req => req.id !== id);
        this.requestService.rejectRequest(id).subscribe();
    }
}
