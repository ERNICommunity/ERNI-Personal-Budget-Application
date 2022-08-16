import { Component, OnInit } from '@angular/core';
import { Request } from '../../model/request/request';
import { RequestService } from '../../services/request.service';
import { ActivatedRoute, Params, Data } from '@angular/router';
import { Observable } from 'rxjs';
import { RequestApprovalState } from '../../model/requestState';
import { ConfigService } from '../../services/config.service';
import { ExportService } from '../../services/export.service';
import { AuthenticationService } from '../../services/authentication.service';
import { AlertService } from '../../services/alert.service';
import { BudgetType } from '../../model/budgetType';
import { BudgetService } from '../../services/budget.service';
import { ApprovalStateModel } from '../../shared/model/approvalStateModel';
import { MenuItem } from 'primeng/api/menuitem';

@Component({
    selector: 'app-request-list',
    templateUrl: 'requestList.component.html',
    styleUrls: ['requestList.component.css']
})
export class RequestListComponent implements OnInit {
    pendingRoute: string = '/requests/pending';
    approvedRoute: string = '/requests/approved';
    completedRoute: string = '/requests/completed';
    rejectedRoute: string = '/requests/rejected';

    requests: Request[];
    filteredRequests: Request[];

    approvalStates: ApprovalStateModel[];
    requestFilter: ApprovalStateModel;
    requestFilterType = RequestApprovalState;

    years: MenuItem[];
    selectedYearItem: MenuItem;
    selectedYear: number;

    budgetTypeMenuItems: MenuItem[];
    selectedBudgetTypeItem: MenuItem;

    approvalStateMenuItems: MenuItem[];
    selectedApprovalState: MenuItem;

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
            {
                state: RequestApprovalState.Pending,
                key: 'pending',
                name: 'Pending'
            },
            {
                state: RequestApprovalState.Approved,
                key: 'approved',
                name: 'Approved'
            },
            {
                state: RequestApprovalState.Rejected,
                key: 'rejected',
                name: 'Rejected'
            }
        ];
    }

    filterRequests(searchString: string) {
        searchString = searchString
            .toLowerCase()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '');

        return this.requests.filter(
            (request) =>
                request.user.firstName
                    .toLowerCase()
                    .normalize('NFD')
                    .replace(/[\u0300-\u036f]/g, '')
                    .indexOf(searchString) !== -1 ||
                request.user.lastName
                    .toLowerCase()
                    .normalize('NFD')
                    .replace(/[\u0300-\u036f]/g, '')
                    .indexOf(searchString.toLowerCase()) !== -1
        );
    }

    constructor(
        private authService: AuthenticationService,
        private budgetService: BudgetService,
        private requestService: RequestService,
        private route: ActivatedRoute,
        //private modalService: NgbModal,
        private exportService: ExportService,
        private alertService: AlertService,
        private config: ConfigService
    ) {
        this.approvalStates = this.getFilter();
        this.requestFilter = this.approvalStates[0];
        console.log(this.requestFilter);
    }

    exportMenuItems: MenuItem[];

    async ngOnInit() {
        this.exportMenuItems = [];
        for (let i = 1; i <= 12; i++) {
            this.exportMenuItems.push({
                label: i.toString(),
                command: (event) => this.export(i, this.selectedYear)
            });
        }

        var currentYear = new Date().getFullYear();

        var budgetTypes = await this.budgetService
            .getBudgetsTypes()
            .toPromise();

        this.route.params.subscribe((params: Params) => {
            var yearParam = params['year'];

            this.selectedBudgetType =
                budgetTypes.find((b) => b.key == params['budgetType']) ??
                budgetTypes[0];

            this.years = [];
            for (
                var year = new Date().getFullYear() + 1;
                year >= this.config.getOldestYear;
                year--
            ) {
                this.years.push({
                    label: year.toString(),
                    routerLink: [
                        '/requests/',
                        this.selectedBudgetType.key,
                        this.requestFilter.key,
                        year
                    ]
                });
            }

            this.selectedYear =
                yearParam != null ? parseInt(yearParam) : currentYear;
            this.selectedYearItem = this.years.find(
                (_) => _.label == this.selectedYear.toString()
            );

            this.requestFilter =
                this.approvalStates.find(
                    (f) => f.key == params['requestState']
                ) ?? this.approvalStates[0];

            this.budgetTypeMenuItems = budgetTypes.map((budgetType) => ({
                id: budgetType.key,
                label: budgetType.name,
                routerLink: [
                    '/requests/',
                    budgetType.key,
                    this.requestFilter.key,
                    this.selectedYear
                ]
            }));
            this.selectedBudgetTypeItem = this.budgetTypeMenuItems.find(
                (t) => t.id == params.requestType
            );

            let approvalStates = this.getFilter();

            this.approvalStateMenuItems = approvalStates.map(
                (approvalState) => ({
                    id: approvalState.key,
                    label: approvalState.name,
                    routerLink: [
                        '/requests/',
                        params.budgetType,
                        approvalState.key,
                        this.selectedYear
                    ]
                })
            );
            this.selectedApprovalState = this.approvalStateMenuItems.find(
                (t) => t.id == params.requestState
            );

            this.getRequests(
                this.requestFilter.state,
                this.selectedYear,
                this.selectedBudgetType.id
            );
        });
    }

    getRequests(
        filter: RequestApprovalState,
        year: number,
        budgetTypeId: number
    ): void {
        var requests: Observable<Request[]>;

        switch (filter) {
            case RequestApprovalState.Approved:
                requests = this.requestService.getApprovedRequests(
                    year,
                    budgetTypeId
                );
                break;
            case RequestApprovalState.Pending:
                requests = this.requestService.getPendingRequests(
                    year,
                    budgetTypeId
                );
                break;
            case RequestApprovalState.Rejected:
                requests = this.requestService.getRejectedRequests(
                    year,
                    budgetTypeId
                );
                break;
            default:
                break;
        }

        requests.subscribe((requests) => {
            (this.requests = requests), (this.filteredRequests = this.requests);
        });
    }

    async completeRequest(request: Request) {
        if (request.invoiceCount < 1) {
            this.alertService.error(
                'Cannot complete request without any invoice attached'
            );
            return;
        }

        try {
            this.requestService.approveRequest(request.id).subscribe(() => {
                (this.requests = this.requests.filter(
                    (req) => req.id !== request.id
                )),
                    (this.filteredRequests = this.requests);
            });

            this.requests = this.requests.filter(
                (req) => req.id !== request.id
            );
            this.filteredRequests = this.requests;
        } catch (error) {
            this.alertService.error(JSON.stringify(error.error));
        }
    }

    rejectRequest(id: number): void {
        this.requestService.rejectRequest(id).subscribe(() => {
            (this.requests = this.requests.filter((req) => req.id !== id)),
                (this.filteredRequests = this.requests);
        });
    }

    canRejectRequest(id: number): boolean {
        return (
            this.authService.userInfo.isAdmin &&
            this.requestFilter.state != RequestApprovalState.Rejected
        );
    }

    export(month: number, year: number) {
        this.exportService.downloadExport(month, year);
    }

    openDeleteConfirmationModal(content) {
        //this.modalService.open(content, { centered: true, backdrop: 'static' });
    }

    deleteRequest(id: number): void {
        this.requestService.deleteRequest(id).subscribe(() => {
            (this.requests = this.requests.filter((req) => req.id !== id)),
                (this.filteredRequests = this.requests);
        });
    }
}
