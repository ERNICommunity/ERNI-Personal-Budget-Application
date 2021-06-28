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
import { BudgetType } from '../../model/budgetType';
import { BudgetService } from '../../services/budget.service';
import { ApprovalStateModel } from '../../shared/model/ApprovalStateModel';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';

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

    budgetTypes: BudgetType[];

    requests: Request[];
    filteredRequests: Request[];

    approvalStates: ApprovalStateModel[];
    requestFilter: ApprovalStateModel;
    requestFilterType = RequestApprovalState;

    selectedYear: number;
    currentYear: number;
    years: number[];
    rlao: object;
    selectedBudgetType: BudgetType;

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

    getFilter(): ApprovalStateModel[] {
        return [
            { state: RequestApprovalState.Pending, key: "pending", name: "Pending" },
            { state: RequestApprovalState.Approved, key: "approved", name: "Approved" },
            { state: RequestApprovalState.Completed, key: "completed", name: "Completed" },
            { state: RequestApprovalState.Rejected, key: "rejected", name: "Rejected" },
        ]
    }

    filterRequests(searchString: string) {
        searchString = searchString.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "");

        return this.requests.filter(request => request.user.firstName.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString) !== -1 ||
            request.user.lastName.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString.toLowerCase()) !== -1);
    }

    constructor(private authService: AuthenticationService,
        private budgetService: BudgetService,
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
        this.approvalStates = this.getFilter();
        this.requestFilter = this.approvalStates[0];
        console.log(this.requestFilter);
    }

    async ngOnInit() {
        var types = await this.budgetService.getBudgetsTypes().toPromise();
        this.budgetTypes = types;
        this.selectedBudgetType = this.budgetTypes[0];

        this.route.params.subscribe((params: Params) => {
            var yearParam = params['year'];

            this.selectedBudgetType = this.budgetTypes.find(b => b.key == params['budgetType']) ?? this.budgetTypes[0];

            this.selectedYear = yearParam != null ? parseInt(yearParam) : this.currentYear;

            this.requestFilter = 
                this.approvalStates.find(f => f.key == params['requestState']) ?? this.approvalStates[0];

            this.getRequests(this.requestFilter.state, this.selectedYear, this.selectedBudgetType.id);
        });
    }

    getRequests(filter: RequestApprovalState, year: number, budgetTypeId: number): void {
        var requests: Observable<Request[]>;

        switch (filter) {
            case RequestApprovalState.Approved:
                requests = this.requestService.getApprovedRequests(year, budgetTypeId);
                break;
            case RequestApprovalState.Pending:
                requests = this.requestService.getPendingRequests(year, budgetTypeId);
                break;
            case RequestApprovalState.Rejected:
                requests = this.requestService.getRejectedRequests(year, budgetTypeId);
                break;
            case RequestApprovalState.Completed:
                requests = this.requestService.getCompletedRequests(year, budgetTypeId);
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
        return this.authService.userInfo.isAdmin && this.requestFilter.state != RequestApprovalState.Rejected;
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
