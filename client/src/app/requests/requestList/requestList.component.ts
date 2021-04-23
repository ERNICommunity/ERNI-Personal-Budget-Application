import { Component, OnInit } from '@angular/core';
import { Request } from '../../model/request/request';
import { RequestService } from '../../services/request.service';
import { ActivatedRoute, Params, Data } from '@angular/router';
import { Observable } from 'rxjs';
import { UserService } from '../../services/user.service';
import { RequestApprovalState } from '../../model/requestState';
import { ConfigService } from '../../services/config.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ExportService } from '../../services/export.service';
import { AuthenticationService } from '../../services/authentication.service';
import { AlertService } from '../../services/alert.service';

@Component({
    selector: 'app-request-list',
    templateUrl: 'requestList.component.html',
    styleUrls: ['requestList.component.css']
})
export class RequestListComponent implements OnInit {
    pendingRoute: string = "/requests/pending";
    approvedRoute: string = "/requests/approved";
    completedRoute: string = "/requests/completed";
    rejectedRoute: string = "/requests/rejected";

    requests: Request[];
    filteredRequests: Request[];
    requestFilter: RequestApprovalState;
    requestFilterType = RequestApprovalState;
    selectedYear: number;
    currentYear: number;
    years: number[];
    rlao: object;

    get isAdmin(): boolean {
        return this.authService.userInfo.isAdmin;
    }

    private _searchTerm: string;

    get searchTerm(): string {
        return this._searchTerm;
    }

    set searchTerm(value: string) {
        this._searchTerm = value;
        this.filteredRequests = this.filterRequests(value);
    }

    filterRequests(searchString: string) {
        searchString = searchString.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "");

        return this.requests.filter(request => request.user.firstName.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString) !== -1 ||
            request.user.lastName.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString.toLowerCase()) !== -1);
    }

    constructor(private authService: AuthenticationService,
        private requestService: RequestService,
        private route: ActivatedRoute,
        private modalService: NgbModal,
        private exportService: ExportService,
        private alertService: AlertService
    ) {

        this.years = [];
        this.currentYear = (new Date()).getFullYear();

        // TODO: do not hardcode year
        for (var year = this.currentYear; year >= 2018; year--) {
            this.years.push(year);
        }
    }

    ngOnInit() {
        this.requestFilter = RequestApprovalState.Pending;
        this.route.data.subscribe((data: Data) => {
            this.requestFilter = <RequestApprovalState>data['filter'];
        });

        this.route.params.subscribe((params: Params) => {

            // the following line forces routerLinkActive to update even if the route did nto change
            // see see https://github.com/angular/angular/issues/13865 for futher info
            this.rlao = { dummy: true };

            //var yearParam = this.route.snapshot.paramMap.get('year');
            var yearParam = params['year'];

            this.selectedYear = yearParam != null ? parseInt(yearParam) : this.currentYear;
            this.getRequests(this.requestFilter, this.selectedYear);
        });
    }

    getRequests(filter: RequestApprovalState, year: number): void {
        var requests: Observable<Request[]>;

        switch (filter) {
            case RequestApprovalState.Approved:
                requests = this.requestService.getApprovedRequests(year);
                break;
            case RequestApprovalState.Pending:
                requests = this.requestService.getPendingRequests(year);
                break;
            case RequestApprovalState.Rejected:
                requests = this.requestService.getRejectedRequests(year);
                break;
            case RequestApprovalState.Completed:
                requests = this.requestService.getCompletedRequests(year);
            default:
                break;
        }

        requests.subscribe(requests => { this.requests = requests, this.filteredRequests = this.requests });
    }

    approveRequest(id: number): void {
        this.requestService.approveRequest(id).subscribe(() => { this.requests = this.requests.filter(req => req.id !== id), this.filteredRequests = this.requests });
    }

    async completeRequest(request: Request) {
      if (!request.invoicedAmount) {
        console.log(request);
        console.log(request.invoicedAmount);
        this.alertService.error('Cannot complete request without invoiced amount entered');
        return;
      }

      if (request.invoiceCount < 1) {
        this.alertService.error('Cannot complete request without any invoice attached');
        return;
      }

      try {
        await this.requestService.completeRequest(request.id);

        this.requests = this.requests.filter(req => req.id !== request.id);
        this.filteredRequests = this.requests;

      } catch (error) {
        this.alertService.error(JSON.stringify(error.error));
      }
    }

    rejectRequest(id: number): void {
        this.requestService.rejectRequest(id).subscribe(() => { this.requests = this.requests.filter(req => req.id !== id), this.filteredRequests = this.requests });
    }

    canRejectRequest(id: number): boolean {
        return this.authService.userInfo.isAdmin && this.requestFilter != this.requestFilterType.Rejected;
    }

    export(month: number, year: number) {
        this.exportService.downloadExport(month, year);
    }

    openDeleteConfirmationModal(content) {
        this.modalService.open(content, { centered: true, backdrop: 'static' });
    }

    deleteRequest(id: number): void {
        this.requestService.deleteRequest(id).subscribe(() => {
            this.requests = this.requests.filter(req => req.id !== id),
                this.filteredRequests = this.requests
        });
    }
}
