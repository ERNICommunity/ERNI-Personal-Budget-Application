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
import { concatMap, defaultIfEmpty, map } from 'rxjs/operators';
import { forkJoin, Observable, Subject } from 'rxjs';
import { InvoiceImage } from '../../model/InvoiceImage';
import { Router } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { BasicRequestInfoEditorComponent } from './basic-request-info-editor/basic-request-info-editor.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-request-edit',
  templateUrl: 'requestEdit.component.html',
  standalone: true,
  imports: [SharedModule, BasicRequestInfoEditorComponent, FileListComponent],
})
export class RequestEditComponent {
  // route params as inputs
  budgetId = input<number>();
  requestId = input<number>();

  showDialog = signal(true);
  budgetType = signal<BudgetTypeEnum | null>(null);
  request = signal<Request | null>(null);
  isSaveInProgress = signal(false);
  files = signal<Invoice[]>([]);

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
          this.files.set([]);
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
    this.requestService
      .getRequest(requestId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((request) => this.request.set(request));

    this.invoiceImageService
      .getInvoiceImages(requestId)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        map((names) =>
          names.map<Invoice>((invoice) => ({
            name: invoice.name,
            status: { code: 'saved', id: invoice.id },
          })),
        ),
      )
      .subscribe((files) => this.files.set(files));
  }

  public onNewImageAdded(files: File[]) {
    for (let i = 0; i < files.length; i++) {
      const selectedFile = files[i];

      const im: Invoice = {
        status: { code: 'new', file: selectedFile },
        name: selectedFile.name,
      };
      this.files.update((files) => [...files, im]);

      const requestId = this.requestId();
      if (requestId) {
        this.uploadInvoice(requestId, im, selectedFile);
      }
    }
  }

  private uploadInvoices(requestId: number): Observable<number[] | null> {
    const uploads$ = this.files()
      .filter((invoice): invoice is { status: NewInvoiceStatus; name: string } => invoice.status.code === 'new')
      .map((invoice) => this.uploadInvoice(requestId, invoice, invoice.status.file));

    return forkJoin(uploads$).pipe(defaultIfEmpty(null));
  }

  private uploadInvoice(requestId: number, invoice: Invoice, file: File): Observable<number> {
    const result = new Subject<number>();

    const fileReader = new FileReader();
    fileReader.readAsDataURL(file);
    fileReader.onload = () => {
      if (fileReader.result) {
        const payload: InvoiceImage = {
          requestId: requestId,
          data: fileReader.result.toString().replace('data:', '').replace(/^.+,/, ''),
          filename: invoice.name,
          mimeType: file.type,
        };

        this.updateFiles({
          ...invoice,
          status: { code: 'in-progress', progress: 0 },
        });

        const uploadInfo = this.invoiceImageService.addInvoiceImage(payload);

        uploadInfo.progress.subscribe((progress) =>
          this.updateFiles({
            ...invoice,
            status: { code: 'in-progress', progress },
          }),
        );
        uploadInfo.id.subscribe((id) =>
          this.updateFiles({
            ...invoice,
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

    request$.pipe(concatMap((requestId) => this.uploadInvoices(requestId))).subscribe({
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

  private updateFiles(fileToUpdate: Invoice): void {
    this.files.update((files) => [...files.map((file) => (file.name !== fileToUpdate.name ? file : fileToUpdate))]);
  }
}
