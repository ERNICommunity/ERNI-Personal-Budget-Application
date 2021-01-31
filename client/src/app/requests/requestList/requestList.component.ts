import { Component, OnInit } from '@angular/core';
import { Request } from '../../model/request/request';
import { RequestService } from '../../services/request.service';
import { RequestFilter } from '../requestFilter';
import { ActivatedRoute, Params, Data } from '@angular/router';
import { Observable } from 'rxjs';
import { UserService } from '../../services/user.service';
import { RequestState } from '../../model/requestState';
import { ConfigService } from '../../services/config.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ExportService } from '../../services/export.service';

@Component({
    selector: 'app-request-list',
    templateUrl: 'requestList.component.html',
    styleUrls: ['requestList.component.css']
})
export class RequestListComponent implements OnInit {
    pendingRoute: string = "/requests/pending";
    approvedRoute: string = "/requests/approved";
    approvedBySuperiorRoute: string = "/requests/approved-by-superior";
    rejectedRoute: string = "/requests/rejected";

    isAdmin: boolean;
    isViewer: boolean;
    requests: Request[];
    filteredRequests: Request[];
    requestFilter: RequestFilter;
    requestFilterType = RequestFilter;
    selectedYear: number;
    currentYear: number;
    years: number[];
    rlao: object;

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

    constructor(private requestService: RequestService,
        private userService: UserService,
        private route: ActivatedRoute,
        private modalService: NgbModal,
        private config: ConfigService,
        private exportService: ExportService
    ) {

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
            this.rlao = { dummy: true };

            //var yearParam = this.route.snapshot.paramMap.get('year');
            var yearParam = params['year'];

            this.selectedYear = yearParam != null ? parseInt(yearParam) : this.currentYear;
            this.getRequests(this.requestFilter, this.selectedYear);
        });

        this.userService.getCurrentUser().subscribe(u => {this.isAdmin = u.isAdmin; this.isViewer = u.isViewer});
    }

    getRequests(filter: RequestFilter, year: number): void {
        var requests: Observable<Request[]>;

        switch (filter) {
            case RequestFilter.Approved:
                requests = this.requestService.getApprovedRequests(year);
                break;
            case RequestFilter.ApprovedBySuperior:
                requests = this.requestService.getApprovedBySuperiorRequests(year);
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

        requests.subscribe(requests => { this.requests = requests, this.filteredRequests = this.requests });
    }

    approveRequest(id: number): void {
        this.requestService.approveRequest(id).subscribe(() => { this.requests = this.requests.filter(req => req.id !== id), this.filteredRequests = this.requests });
    }

    rejectRequest(id: number): void {
        this.requestService.rejectRequest(id).subscribe(() => { this.requests = this.requests.filter(req => req.id !== id), this.filteredRequests = this.requests });
    }

    canRejectRequest(id: number): boolean {
        return this.isAdmin && this.requestFilter != this.requestFilterType.Rejected;
    }

    showApprove(): boolean {
        if (this.isAdmin) {
            return this.requestFilter != this.requestFilterType.Approved && this.requestFilter != this.requestFilterType.Rejected;
        }

        return this.requestFilter == this.requestFilterType.Pending;
    }

    export(month: number, year: number) {
        console.log(month);
        console.log(year);
        alert('Exporting ' + month + ' ' + year);
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
