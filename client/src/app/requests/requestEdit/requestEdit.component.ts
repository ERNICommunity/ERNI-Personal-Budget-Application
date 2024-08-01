import { Component, computed, DestroyRef, effect, inject, input, signal, untracked } from '@angular/core';
import { RequestService } from '../../services/request.service';
import { AlertService } from '../../services/alert.service';
import { AlertType } from '../../model/alert.model';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { BudgetTypeEnum } from '../../model/budgetTypeEnum';
import { Request } from '../../model/request/request';
import { PatchRequest } from '../../model/PatchRequest';
import { NewRequest } from '../../model/newRequest';
import { ActivatedRoute } from '@angular/router';
import { RequestApprovalState } from '../../model/requestState';
import { InvoiceImageService } from '../../services/invoice-image.service';
import { FileListComponent, Invoice, NewInvoiceStatus } from '../../shared/file-list/file-list.component';
import { concatMap, defaultIfEmpty, map, tap } from 'rxjs/operators';
import { finalize, forkJoin, Observable, Subject } from 'rxjs';
import { InvoiceImage } from '../../model/InvoiceImage';
import { Router } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-request-edit',
  templateUrl: 'requestEdit.component.html',
  standalone: true,
  imports: [SharedModule, FileListComponent],
})
export class RequestEditComponent {
  // route params as inputs
  budgetId = input<number>();
  requestId = input<number>();

  showDialog = signal(true);
  budgetType = signal<BudgetTypeEnum | null>(null);
  request = signal<Request | null>(null);
  isSaveInProgress = signal(false);
  attachments = signal<Invoice[]>([]);
  isRequestLoading = signal(false);

  isNewRequest = computed(() => !!this.budgetId());

  public RequestState = RequestApprovalState; // this is required to be possible to use enum in view

  private destroyRef = inject(DestroyRef);

  constructor(
    private requestService: RequestService,
    private router: Router,
    private route: ActivatedRoute,
    private alertService: AlertService,
    private dataChangeNotificationService: DataChangeNotificationService,
    private invoiceImageService: InvoiceImageService,
  ) {
    effect(() => {
      const budgetId = this.budgetId();
      const requestId = this.requestId();

      untracked(() => {
        if (budgetId && !isNaN(budgetId)) {
          this.request.set(this.createNewRequest());
          this.attachments.set([]);
        } else if (requestId && !isNaN(requestId)) {
          this.loadRequest(requestId);
        } else {
          this.router.navigate(['my-budget']);
        }
      });
    });
  }

  private createNewRequest(): Request {
    return {
      state: RequestApprovalState.Pending,
      id: 0,
      amount: 0,
      invoiceCount: 0,
      user: undefined!,
      budget: undefined!,
      title: '',
      createDate: undefined!,
    };
  }

  private loadRequest(requestId: number): void {
    this.isRequestLoading.set(true);

    const request$ = this.requestService.getRequest(requestId).pipe(tap((request) => this.request.set(request)));
    const attachments$ = this.invoiceImageService.getInvoiceImages(requestId).pipe(
      map((names) =>
        names.map<Invoice>((invoice) => ({
          name: invoice.name,
          status: { code: 'saved', id: invoice.id },
        })),
      ),
      tap((attachments) => this.attachments.set(attachments)),
    );

    forkJoin({
      request: request$,
      attachments: attachments$,
    })
      .pipe(
        finalize(() => this.isRequestLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe();
  }

  public onNewFileAdded(files: File[]) {
    for (let i = 0; i < files.length; i++) {
      const selectedFile = files[i];

      const attachment: Invoice = {
        status: { code: 'new', file: selectedFile },
        name: selectedFile.name,
      };
      this.attachments.update((files) => [...files, attachment]);

      const requestId = this.requestId();
      if (requestId) {
        this.uploadAttachment(requestId, attachment, selectedFile);
      }
    }
  }

  private uploadAttachments(requestId: number): Observable<number[] | null> {
    const uploads$ = this.attachments()
      .filter(
        (attachment): attachment is { status: NewInvoiceStatus; name: string } => attachment.status.code === 'new',
      )
      .map((attachment) => this.uploadAttachment(requestId, attachment, attachment.status.file));

    return forkJoin(uploads$).pipe(defaultIfEmpty(null));
  }

  private uploadAttachment(requestId: number, attachment: Invoice, file: File): Observable<number> {
    const result = new Subject<number>();

    const fileReader = new FileReader();
    fileReader.readAsDataURL(file);
    fileReader.onload = () => {
      if (fileReader.result) {
        const payload: InvoiceImage = {
          requestId: requestId,
          data: fileReader.result.toString().replace('data:', '').replace(/^.+,/, ''),
          filename: attachment.name,
          mimeType: file.type,
        };

        this.updateAttachmentState({
          ...attachment,
          status: { code: 'in-progress', progress: 0 },
        });

        const uploadInfo = this.invoiceImageService.addInvoiceImage(payload);

        uploadInfo.progress.subscribe((progress) =>
          this.updateAttachmentState({
            ...attachment,
            status: { code: 'in-progress', progress },
          }),
        );
        uploadInfo.id.subscribe((id) =>
          this.updateAttachmentState({
            ...attachment,
            status: { code: 'saved', id },
          }),
        );

        uploadInfo.id.subscribe(result);
      }
    };

    return result;
  }

  public save() {
    const request = this.request();
    if (!request || !request.title) {
      return;
    }

    if (request.state == RequestApprovalState.Pending) {
      this.saveBasicInfo(request);
    }
  }

  private saveBasicInfo(request: Request) {
    const budgetId = this.budgetId();
    const requestId = this.requestId();
    const title: string = request.title;
    const amount: number = request.amount;

    this.isSaveInProgress.set(true);

    if (budgetId) {
      this.saveNewRequest({
        budgetId,
        title,
        amount,
      });
    } else if (requestId && request.state == RequestApprovalState.Pending) {
      this.editExistingRequest({
        id: requestId,
        title,
        amount,
      });
    }
  }

  private saveNewRequest(payload: NewRequest): void {
    const request$ =
      // FIXME when is budgetType set? I don't see any assignment anywhere?
      this.budgetType() == BudgetTypeEnum.TeamBudget
        ? this.requestService.addTeamRequest(payload)
        : this.requestService.addRequest(payload);

    request$.pipe(concatMap((requestId) => this.uploadAttachments(requestId))).subscribe({
      next: (_) => {
        this.isSaveInProgress.set(false);
        this.dataChangeNotificationService.notify();
        this.alertService.alert({
          message: 'Request created successfully',
          type: AlertType.Success,
          life: 3_000,
          keepAfterRouteChange: true,
        });

        this.router.navigate(['../../'], { relativeTo: this.route });
      },
      error: (err) => {
        this.dataChangeNotificationService.notify();
        this.isSaveInProgress.set(false);
        this.alertService.error('Error while creating request: ' + JSON.stringify(err), 'addRequestError');
      },
    });
  }

  private editExistingRequest(payload: PatchRequest): void {
    const request$ =
      this.budgetType() == BudgetTypeEnum.TeamBudget
        ? this.requestService.updateTeamRequest(payload)
        : this.requestService.updateRequest(payload);

    request$.subscribe({
      next: () => {
        this.isSaveInProgress.set(false);
        this.dataChangeNotificationService.notify();
        this.alertService.alert({
          message: 'Request updated',
          type: AlertType.Success,
          life: 3_000,
          keepAfterRouteChange: true,
        });
        this.router.navigate(['../../'], { relativeTo: this.route });
      },
      error: (err) => {
        this.dataChangeNotificationService.notify();
        this.isSaveInProgress.set(false);
        this.alertService.error('Error while creating request: ' + JSON.stringify(err.error), 'addRequestError');
      },
    });
  }

  public onHide(): void {
    this.router.navigate(['../../'], { relativeTo: this.route });
  }

  /**
   * Updates local storage of list of attachments with updated attachment.
   * @param updatedAttachment
   */
  private updateAttachmentState(updatedAttachment: Invoice): void {
    this.attachments.update((_) => [
      ..._.map((attachment) => (attachment.name !== updatedAttachment.name ? attachment : updatedAttachment)),
    ]);
  }
}
