import {Component, OnInit} from '@angular/core';
import {Request} from '../../model/request';
import {RequestService} from '../../services/request.service';
import { RequestFilter } from '../requestFilter';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Component({
    selector: 'app-request-list',
    templateUrl: 'requestList.component.html',
    styleUrls: ['requestList.component.css']
})
export class RequestListComponent implements OnInit {
    pendingRoute: string = "/requests/pending";
    approvedRoute: string= "/requests/approved";
    rejectedRoute: string= "/requests/rejected";

    requests: Request[];
    requestFilter : RequestFilter;
    requestFilterType = RequestFilter;
    selectedYear: number;
    currentYear: number;
    years : number[];

    constructor(private requestService: RequestService, private route: ActivatedRoute, private router: Router) {
        this.years = []; 
        this.currentYear = (new Date()).getFullYear();
                
        for (var year = 2008; year <= this.currentYear + 1; year++) {
             this.years.push(year);
        }
    }

    ngOnInit() {
        this.requestFilter = <RequestFilter>this.route.snapshot.data['filter'];
                
        var yearParam = this.route.snapshot.paramMap.get('year');
        this.selectedYear = yearParam != null ? parseInt(yearParam) : this.currentYear;
        this.getRequests(this.requestFilter, this.selectedYear);
    }

    getRequests(filter: RequestFilter, year: number): void {
        var requests: Observable<Request[]>;

        switch (filter) {
            case RequestFilter.Approved:
                requests = this.requestService.getApprovedRequests(year);
                break;
            case RequestFilter.Pending:
                requests = this.requestService.getPendingRequests(year);
                break;
            case RequestFilter.Rejected:
                requests = this.requestService.getRejectedRequests(year);
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
