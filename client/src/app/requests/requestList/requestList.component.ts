import { Component, OnDestroy, OnInit } from '@angular/core';
import { Request } from '../../model/request/request';
import { RequestService } from '../../services/request.service';
import { ActivatedRoute } from '@angular/router';
import {
    BehaviorSubject,
    combineLatest,
    merge,
    Observable,
    ReplaySubject,
    Subject,
    Subscription
} from 'rxjs';
import { RequestApprovalState } from '../../model/requestState';
import { ExportService } from '../../services/export.service';
import { AuthenticationService } from '../../services/authentication.service';
import { AlertService } from '../../services/alert.service';
import { BudgetType } from '../../model/budgetType';
import { BudgetService } from '../../services/budget.service';
import { MenuItem } from 'primeng/api/menuitem';
import { ConfirmationService } from 'primeng/api';
import { distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { MenuHelper } from '../../shared/menu-helper';
import {
    maintainList,
    RemoveEvent,
    ResetEvent
} from '../../shared/utils/observableUtils';

@Component({
    selector: 'app-request-list',
    templateUrl: 'requestList.component.html',
    styleUrls: ['requestList.component.css'],
    providers: [ConfirmationService]
})
export class RequestListComponent implements OnInit, OnDestroy {
    pendingRoute = '/requests/pending';
    approvedRoute = '/requests/approved';
    completedRoute = '/requests/completed';
    rejectedRoute = '/requests/rejected';

    requests$: Observable<Request[]>;

    years$: Observable<MenuItem[]>;
    budgetTypeMenuItems$: Observable<MenuItem[]>;
    approvalStateMenuItems$: Observable<MenuItem[]>;
    exportMenuItems$: Observable<MenuItem[]>;

    approvalStates = RequestApprovalState;

    isAdmin: boolean;

    states: {
        state: RequestApprovalState;
        key: 'approved' | 'rejected' | 'pending';
        name: string;
    }[] = [
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

    removeEvent$ = new Subject<ResetEvent<Request> | RemoveEvent>();

    private _searchTerm$ = new BehaviorSubject<string>('');
    subscription: Subscription;

    get searchTerm(): string {
        return this._searchTerm$.value;
    }

    set searchTerm(value: string) {
        this._searchTerm$.next(value);
    }

    filterRequests(requests: Request[], searchString: string): Request[] {
        searchString = searchString
            .toLowerCase()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '');

        return requests.filter(
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
        private confirmationService: ConfirmationService,
        private exportService: ExportService,
        private alertService: AlertService
    ) {}

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }

    async ngOnInit() {
        this.subscription = this.authService.userInfo$.subscribe(
            (_) => (this.isAdmin = !!_ && _.isAdmin)
        );

        const selectedYear$ = this.route.params.pipe(
            map((params) => +params['year']),
            distinctUntilChanged()
        );

        const selectedBudgetType$ = this.route.params.pipe(
            map((params) => +params['budgetType']),
            distinctUntilChanged()
        );

        const requestState$ = this.route.params.pipe(
            map((params) => '' + params['requestState']),
            distinctUntilChanged()
        );

        const budgetTypes$ = new ReplaySubject<BudgetType[]>();
        this.budgetService.getBudgetsTypes().subscribe(budgetTypes$);

        this.exportMenuItems$ = selectedYear$.pipe(
            map((year) => {
                const list = [];
                for (let i = 1; i <= 12; i++) {
                    list.push({
                        label: i.toString(),
                        command: () => this.export(i, year)
                    });
                }
                return list;
            })
        );

        this.years$ = combineLatest([selectedBudgetType$, requestState$]).pipe(
            map(([budgetType, requestState]) =>
                MenuHelper.getYearMenu((year) => [
                    '/requests/',
                    budgetType,
                    requestState,
                    year
                ])
            )
        );

        this.approvalStateMenuItems$ = combineLatest([
            selectedBudgetType$,
            selectedYear$
        ]).pipe(
            map(([budgetType, year]) =>
                this.states.map((approvalState) => ({
                    id: approvalState.key,
                    label: approvalState.name,
                    routerLink: [
                        '/requests/',
                        budgetType,
                        approvalState.key,
                        year
                    ]
                }))
            )
        );

        this.budgetTypeMenuItems$ = combineLatest([
            budgetTypes$,
            selectedYear$,
            requestState$
        ]).pipe(
            map(([budgetTypes, selectedYear, requestState]) =>
                budgetTypes.map((budgetType) => ({
                    id: budgetType.key,
                    label: budgetType.name,
                    routerLink: [
                        '/requests/',
                        budgetType.id,
                        requestState,
                        selectedYear
                    ]
                }))
            )
        );

        const requests$ = combineLatest([
            selectedBudgetType$,
            selectedYear$,
            requestState$
        ]).pipe(
            switchMap(([budgetType, year, state]) =>
                this.requestService.getRequests(
                    year,
                    this.states.find((_) => _.key === state).key,
                    budgetType
                )
            ),
            map((requests) =>
                requests.sort((a, b) => b.invoiceCount - a.invoiceCount)
            ),
            map(
                (requests) =>
                    ({ list: requests } as ResetEvent<Request> | RemoveEvent)
            )
        );

        const maintainedRequests = maintainList(
            merge(requests$, this.removeEvent$.asObservable())
        );

        this.requests$ = combineLatest([
            maintainedRequests,
            this._searchTerm$
        ]).pipe(
            map(([requests, searchString]) =>
                this.filterRequests(requests, searchString)
            )
        );
    }

    async completeRequest(request: Request) {
        if (request.invoiceCount < 1) {
            this.alertService.error(
                'Cannot complete request without any invoice attached'
            );
            return;
        }
        this.requestService
            .approveRequest(request.id)
            .subscribe(() => this.removeEvent$.next({ id: request.id }));
    }

    export(month: number, year: number) {
        this.exportService.downloadExport(month, year);
    }

    openDeleteConfirmationModal(request: Request) {
        this.confirmationService.confirm({
            message: `Are you sure you want to delete the request "${request.title}"?`,
            header: 'Delete Confirmation',
            icon: 'pi pi-info-circle',
            accept: () => {
                this.requestService
                    .deleteRequest(request.id)
                    .subscribe(() =>
                        this.removeEvent$.next({ id: request.id })
                    );
            }
        });
    }

    openRejectConfirmationModal(request: Request) {
        this.confirmationService.confirm({
            message: `Are you sure you want to reject the request "${request.title}"?`,
            header: 'Delete Confirmation',
            icon: 'pi pi-info-circle',
            accept: () => {
                this.requestService
                    .rejectRequest(request.id)
                    .subscribe(() =>
                        this.removeEvent$.next({ id: request.id })
                    );
            }
        });
    }
}
