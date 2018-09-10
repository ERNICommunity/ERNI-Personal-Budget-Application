import {Component, OnInit} from '@angular/core';
import {Request} from '../../model/request';
import {RequestService} from '../../services/request.service';
import { RequestFilter } from '../requestFilter';
import { ActivatedRoute, Params, Data } from '@angular/router';
import { Observable } from 'rxjs';
import { UserService } from '../../services/user.service';
import { RequestState } from '../../model/requestState';
import { ConfigService } from '../../services/config.service';

@Component({
    selector: 'app-request-list',
    templateUrl: 'requestList.component.html',
    styleUrls: ['requestList.component.css']
})
export class RequestListComponent implements OnInit {
    pendingRoute: string = "/requests/pending";
    approvedRoute: string= "/requests/approved";
    rejectedRoute: string= "/requests/rejected";

    isAdmin: boolean;
    requests: Request[];
    requestFilter : RequestFilter;
    requestFilterType = RequestFilter;
    selectedYear: number;
    currentYear: number;
    years : number[];
    rlao: object;

    constructor(private requestService: RequestService, private userService: UserService, private route: ActivatedRoute, private config: ConfigService) {
        this.years = []; 
        this.currentYear = (new Date()).getFullYear();
                
        for (var year = this.currentYear; year >= config.getOldestYear; year--) {
             this.years.push(year);
        }
    }

    ngOnInit() {
        this.requestFilter = RequestFilter.Pending;
         this.route.data.subscribe((data: Data) => {
            this.requestFilter = <RequestFilter>data['filter'];
         });

        this.route.params.subscribe((params: Params) => {

            // the following line forces routerLinkActive to update even if the route did nto change
            // see see https://github.com/angular/angular/issues/13865 for futher info
            this.rlao = {dummy: true};

            //var yearParam = this.route.snapshot.paramMap.get('year');
            var yearParam = params['year']; 

            this.selectedYear = yearParam != null ? parseInt(yearParam) : this.currentYear;
            this.getRequests(this.requestFilter, this.selectedYear);
          });

          this.userService.getCurrentUser().subscribe(u => this.isAdmin = u.isAdmin);
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

    canRejectRequest(id: number): boolean {
        if (this.isAdmin) {
            return this.requestFilter != this.requestFilterType.Rejected;
        }

        var request = this.requests.find(req => req.id == id);
        return this.requestFilter != this.requestFilterType.Rejected && request.state != RequestState.Approved;
    }
}
