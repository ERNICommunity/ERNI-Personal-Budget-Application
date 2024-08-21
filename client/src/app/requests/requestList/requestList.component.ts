import { Component, OnDestroy, OnInit, computed, effect, inject, model, signal } from '@angular/core';
import { Request } from '../../model/request/request';
import { RequestService } from '../../services/request.service';
import { ActivatedRoute } from '@angular/router';
import { firstValueFrom, Subject, Subscription } from 'rxjs';
import { RequestApprovalState } from '../../model/requestState';
import { ExportService } from '../../services/export.service';
import { AuthenticationService } from '../../services/authentication.service';
import { AlertService } from '../../services/alert.service';
import { BudgetService } from '../../services/budget.service';
import { ConfirmationService } from 'primeng/api';
import { distinctUntilChanged, map } from 'rxjs/operators';
import { MenuHelper } from '../../shared/menu-helper';
import { RemoveEvent, ResetEvent } from '../../shared/utils/observableUtils';
import { filterRequests } from '../../utils/filters';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-request-list',
  templateUrl: 'requestList.component.html',
  styleUrls: ['requestList.component.css'],
  providers: [ConfirmationService],
})
export class RequestListComponent implements OnInit, OnDestroy {
  authService = inject(AuthenticationService);
  budgetService = inject(BudgetService);
  requestService = inject(RequestService);
  route = inject(ActivatedRoute);
  confirmationService = inject(ConfirmationService);
  exportService = inject(ExportService);
  alertService = inject(AlertService);

  pendingRoute = '/requests/pending';
  approvedRoute = '/requests/approved';
  completedRoute = '/requests/completed';
  rejectedRoute = '/requests/rejected';

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
      name: 'Pending',
    },
    {
      state: RequestApprovalState.Approved,
      key: 'approved',
      name: 'Approved',
    },
    {
      state: RequestApprovalState.Rejected,
      key: 'rejected',
      name: 'Rejected',
    },
  ];

  #selectedYear = toSignal(
    this.route.params.pipe(
      map((params) => +params['year']),
      distinctUntilChanged(),
    ),
  );

  #selectedBudgetType = toSignal(
    this.route.params.pipe(
      map((params) => +params['budgetType']),
      distinctUntilChanged(),
    ),
  );

  #selectedRequestState = toSignal(
    this.route.params.pipe(
      map((params) => '' + params['requestState']),
      distinctUntilChanged(),
    ),
  );

  #budgetTypes = toSignal(this.budgetService.getBudgetsTypes());

  exportMenuItems = computed(() => {
    const selectedYear = this.#selectedYear();

    if (!selectedYear) {
      return [];
    }
    const list = [];
    for (let i = 1; i <= 12; i++) {
      list.push({
        label: i.toString(),
        command: () => this.export(i, selectedYear),
      });
    }
    return list;
  });

  yearMenuItems = computed(() => {
    const selectedBudgetType = this.#selectedBudgetType();
    const selectedRequestState = this.#selectedRequestState();

    if (!selectedBudgetType || !selectedRequestState) {
      return [];
    }

    return MenuHelper.getYearMenu((year) => ['/requests/', selectedBudgetType, selectedRequestState, year]);
  });

  approvalStateMenuItems = computed(() => {
    const selectedBudgetType = this.#selectedBudgetType();
    const selectedYear = this.#selectedYear();

    if (!selectedBudgetType || !selectedYear) {
      return [];
    }
    return this.states.map((approvalState) => ({
      id: approvalState.key,
      label: approvalState.name,
      routerLink: ['/requests/', selectedBudgetType, approvalState.key, selectedYear],
    }));
  });

  budgetTypeMenuItems = computed(() => {
    const selectedRequestState = this.#selectedRequestState();
    const selectedYear = this.#selectedYear();

    const budgetTypes = this.#budgetTypes();

    if (!selectedRequestState || !selectedYear || !budgetTypes) {
      return [];
    }

    return budgetTypes.map((budgetType) => ({
      id: budgetType.key,
      label: budgetType.name,
      routerLink: ['/requests/', budgetType.id, selectedRequestState, selectedYear],
    }));
  });

  requests = signal<Request[]>([]);

  requestsEffect = effect(async () => {
    const selectedYear = this.#selectedYear();
    const selectedRequestState = this.#selectedRequestState();
    const selectedBudgetType = this.#selectedBudgetType();

    if (!selectedYear || !selectedRequestState || !selectedBudgetType) {
      return;
    }

    const requests = await firstValueFrom(
      this.requestService
        .getRequests(selectedYear, this.states.find((_) => _.key === selectedRequestState)!.key, selectedBudgetType)
        .pipe(map((requests) => requests.sort((a, b) => b.invoiceCount - a.invoiceCount))),
    );

    this.requests.set(requests);
  });

  filteredRequests = computed(() =>
    filterRequests(this.requests(), this.searchTerm() ?? '' /* WHY CAN THIS BE UNDEFINED? */),
  );

  removeEvent$ = new Subject<ResetEvent<Request> | RemoveEvent>();

  subscription: Subscription;

  searchTerm = model('');

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  async ngOnInit() {
    this.subscription = this.authService.userInfo$.subscribe((_) => (this.isAdmin = !!_ && _.isAdmin));
  }

  async completeRequest(request: Request) {
    if (request.invoiceCount < 1) {
      this.alertService.error('Cannot complete request without any invoice attached');
      return;
    }
    this.requestService.approveRequest(request.id).subscribe(() => this.removeEvent$.next({ id: request.id }));
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
        this.requestService.deleteRequest(request.id).subscribe(() => this.removeEvent$.next({ id: request.id }));
      },
    });
  }

  openRejectConfirmationModal(request: Request) {
    this.confirmationService.confirm({
      message: `Are you sure you want to reject the request "${request.title}"?`,
      header: 'Delete Confirmation',
      icon: 'pi pi-info-circle',
      accept: () => {
        this.requestService.rejectRequest(request.id).subscribe(() => this.removeEvent$.next({ id: request.id }));
      },
    });
  }
}
