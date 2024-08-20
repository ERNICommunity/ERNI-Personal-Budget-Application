import {
  ChangeDetectionStrategy,
  Component,
  computed,
  DestroyRef,
  effect,
  inject,
  input,
  signal,
  untracked,
} from '@angular/core';
import { RequestService } from '../../services/request.service';
import { AlertService } from '../../services/alert.service';
import { AlertType } from '../../model/alert.model';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { BudgetTypeEnum } from '../../model/budgetTypeEnum';
import { Request } from '../../model/request/request';
import { PatchRequest } from '../../model/PatchRequest';
import { NewRequest } from '../../model/newRequest';
import { ActivatedRoute, Router } from '@angular/router';
import { RequestApprovalState } from '../../model/requestState';
import { InvoiceImageService } from '../../services/invoice-image.service';
import { FileListComponent, Invoice, NotUploadedInvoice } from '../../shared/file-list/file-list.component';
import { catchError, concatMap, defaultIfEmpty, filter, map, min, switchMap, tap } from 'rxjs/operators';
import { finalize, forkJoin, Observable, of } from 'rxjs';
import { InvoiceImage } from '../../model/InvoiceImage';
import { SharedModule } from '../../shared/shared.module';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { readFile } from '../../shared/utils/rx-file-reader';

@Component({
  selector: 'app-request-edit',
  templateUrl: 'requestEdit.component.html',
  standalone: true,
  imports: [SharedModule, FileListComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
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
          status: 'uploaded',
          id: invoice.id,
          name: invoice.name,
        })),
      ),
      tap((attachments) => this.attachments.set(attachments)),
    );

    forkJoin([request$, attachments$])
      .pipe(
        finalize(() => this.isRequestLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe();
  }

  public onNewFileAdded(files: File[]) {
    const newAttachments: Invoice[] = files.map((file) => ({
      status: 'not-uploaded',
      file,
      name: file.name,
    }));
    this.attachments.update((attachments) => [...attachments, ...newAttachments]);

    // when we add attachments to existing request then we upload them right away
    const requestId = this.requestId();
    if (requestId) {
      this.uploadAttachments(requestId, newAttachments).subscribe();
    }
  }

  private uploadAttachments(requestId: number, attachments: Invoice[]): Observable<number[]> {
    const uploadAttachmentTasks = attachments
      .filter((attachment): attachment is NotUploadedInvoice => attachment.status === 'not-uploaded')
      .map((attachment) =>
        this.uploadAttachment(requestId, attachment, attachment.file).pipe(
          catchError((err) => {
            const error = 'message' in err ? err.message : JSON.stringify(err);
            this.alertService.error('Error during uploading attachment: ' + JSON.stringify(error), 'addRequestError');

            // in case of error during upload we remove the problematic file from the list
            this.attachments.update((attachments) => attachments.filter((f) => f.name !== attachment.name));

            return of(null);
          }),
        ),
      );

    return forkJoin(uploadAttachmentTasks).pipe(
      map((results) => results.filter((result): result is number => !!result)),
      defaultIfEmpty([]),
    );
  }

  private uploadAttachment(requestId: number, attachment: Invoice, file: File): Observable<number> {
    return readFile(file).pipe(
      map((fileReaderResult): InvoiceImage => {
        if (!fileReaderResult) {
          throw new Error(`File '${file.name}' was not able to be read.`);
        }
        return {
          requestId: requestId,
          data: fileReaderResult.toString().replace('data:', '').replace(/^.+,/, ''),
          filename: attachment.name,
          mimeType: file.type,
        };
      }),
      tap(() => {
        this.updateAttachmentState({
          ...attachment,
          status: 'uploading',
          progress: 0,
        });
      }),
      switchMap((payload) =>
        this.invoiceImageService.addInvoiceImage(payload).pipe(
          tap((result) => {
            if (result.status === 'in-progress') {
              this.updateAttachmentState({
                ...attachment,
                status: 'uploading',
                progress: result.progress,
              });
            } else {
              this.updateAttachmentState({
                ...attachment,
                status: 'uploaded',
                id: result.id,
              });
            }
          }),
          filter((result) => result.status === 'completed'),
          map((result) => result.id),
        ),
      ),
    );
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

    this.isSaveInProgress.set(true);

    request$
      .pipe(
        concatMap((requestId) => this.uploadAttachments(requestId, this.attachments())),
        finalize(() => {
          this.isSaveInProgress.set(false);
          this.dataChangeNotificationService.notify();
        }),
      )
      .subscribe({
        next: (_) => {
          this.alertService.alert({
            message: 'Request created successfully',
            type: AlertType.Success,
            life: 3_000,
            keepAfterRouteChange: true,
          });

          this.router.navigate(['../../'], { relativeTo: this.route });
        },
        error: (err) => {
          const error = 'message' in err ? err.message : JSON.stringify(err);
          this.alertService.error('Error while creating request: ' + JSON.stringify(error), 'addRequestError');
        },
      });
  }

  private editExistingRequest(payload: PatchRequest): void {
    const request$ =
      this.budgetType() == BudgetTypeEnum.TeamBudget
        ? this.requestService.updateTeamRequest(payload)
        : this.requestService.updateRequest(payload);

    this.isSaveInProgress.set(true);

    request$
      .pipe(
        finalize(() => {
          this.isSaveInProgress.set(false);
          this.dataChangeNotificationService.notify();
        }),
      )
      .subscribe({
        next: () => {
          this.alertService.alert({
            message: 'Request updated',
            type: AlertType.Success,
            life: 3_000,
            keepAfterRouteChange: true,
          });
          this.router.navigate(['../../'], { relativeTo: this.route });
        },
        error: (err) => {
          const error = 'message' in err ? err.message : JSON.stringify(err);
          this.alertService.error('Error while editing existing request: ' + JSON.stringify(error), 'addRequestError');
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

  protected readonly min = min;
}
