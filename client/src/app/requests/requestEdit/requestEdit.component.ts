import { Component, OnInit } from '@angular/core';
import { RequestService } from '../../services/request.service';
import { AlertService } from '../../services/alert.service';
import { Alert, AlertType } from '../../model/alert.model';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { BudgetTypeEnum } from '../../model/budgetTypeEnum';
import { Request } from '../../model/request/request';
import { PatchRequest } from '../../model/PatchRequest';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { NewRequest } from '../../model/newRequest';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { RequestApprovalState } from '../../model/requestState';
import { InvoiceImageService } from '../../services/invoice-image.service';
import { Invoice, InvoiceStatus, FileListComponent } from '../../shared/file-list/file-list.component';
import { concatMap, defaultIfEmpty } from 'rxjs/operators';
import { forkJoin, Observable, Subject } from 'rxjs';
import { InvoiceImage } from '../../model/InvoiceImage';
import { DividerModule } from 'primeng/divider';
import { ButtonModule } from 'primeng/button';
import { NgIf } from '@angular/common';
import { BasicRequestInfoEditorComponent } from './basic-request-info-editor/basic-request-info-editor.component';
import { AlertComponent } from '../../shared/alert/alert.component';
import { DialogModule } from 'primeng/dialog';

@Component({
    selector: 'app-request-edit',
    templateUrl: 'requestEdit.component.html',
    styleUrls: ['requestEdit.component.css'],
    standalone: true,
    imports: [DialogModule, AlertComponent, BasicRequestInfoEditorComponent, NgIf, ButtonModule, FileListComponent, DividerModule]
})
export class RequestEditComponent implements OnInit {
    httpResponseError: string;
    requestId: number;

    popupTitle: string;
    isVisible: boolean;

    budgetId: number;
    budgetType: BudgetTypeEnum;
    newRequest: boolean;
    request: Request;

    title: string;
    amount: number;

    public isSaveInProgress = false;

    public RequestState = RequestApprovalState; // this is required to be possible to use enum in view
    public images: (Invoice & { file?: File })[];

    constructor(
        private requestService: RequestService,
        private router: Router,
        private route: ActivatedRoute,
        private alertService: AlertService,
        private dataChangeNotificationService: DataChangeNotificationService,
        private busyIndicatorService: BusyIndicatorService,
        private invoiceImageService: InvoiceImageService
    ) {}

    ngOnInit() {
        this.isVisible = true;

        this.route.params.subscribe((params: Params) => {
            this.budgetId = Number(params['budgetId']);
            this.requestId = Number(params['requestId']);

            if (!isNaN(this.budgetId)) {
                this.popupTitle = 'Create new request';
                this.request = this.createNewRequest();
                this.newRequest = true;
                this.images = [];
            } else if (!isNaN(this.requestId)) {
                this.popupTitle = 'Request details';
                this.loadRequest(this.requestId);
                this.newRequest = false;
            } else {
                this.router.navigate(['my-budget']);
            }
        });
    }

    private createNewRequest(): Request {
        var request = new Request();
        request.state = RequestApprovalState.Pending;
        return request;
    }

    public loadRequest(requestId: number): void {
        this.requestId = requestId;
        this.requestService.getRequest(requestId).subscribe(
            (request) => {
                this.request = {
                    amount: request.amount,
                    budget: request.budget,
                    createDate: request.createDate,
                    id: request.id,
                    state: request.state,
                    title: request.title,
                    user: request.user,
                    invoiceCount: request.invoiceCount
                };
            },
            (err) => {
                this.httpResponseError = err.error;
            }
        );

        this.invoiceImageService.getInvoiceImages(this.requestId).subscribe(
            (names) =>
                (this.images = names.map((invoice) => ({
                    name: invoice.name,
                    status: { code: 'saved', id: invoice.id }
                })))
        );
    }

    public onNewImageAdded(files: FileList) {
        for (var i = 0; i < files.length; i++) {
            const selectedFile = files[i];

            const im: Invoice & { file: File } = {
                status: { code: 'new' },
                name: selectedFile.name,
                file: selectedFile
            };
            this.images.push(im);

            if (!this.newRequest) {
                this.uploadInvoice(this.requestId, im, selectedFile);
            }
        }
    }

    private uploadInvoices(requestId: number): Observable<any> {
        const invoices = this.images;
        const uploads = invoices
            .filter((_) => _.status.code === 'new')
            .map((_) => this.uploadInvoice(requestId, _, _.file).id);

        return forkJoin(uploads).pipe(defaultIfEmpty(null));
    }

    private uploadInvoice(
        requestId: number,
        invoice: Invoice,
        file: File
    ): {
        progress: Observable<number>;
        id: Observable<number>;
    } {
        const result = {
            progress: new Subject<number>(),
            id: new Subject<number>()
        };

        const fileReader = new FileReader();
        fileReader.readAsDataURL(file);
        fileReader.onload = () => {
            if (fileReader.result) {
                const payload: InvoiceImage = {
                    requestId: requestId,
                    data: fileReader.result
                        .toString()
                        .replace('data:', '')
                        .replace(/^.+,/, ''),
                    filename: invoice.name,
                    mimeType: file.type
                };

                const status: InvoiceStatus = {
                    code: 'in-progress',
                    progress: 0
                };

                invoice.status = status;

                var uploadInfo =
                    this.invoiceImageService.addInvoiceImage(payload);

                uploadInfo.progress.subscribe(
                    (progress) => (status.progress = progress)
                );
                uploadInfo.id.subscribe((id) => {
                    const status: InvoiceStatus = (invoice.status = {
                        code: 'saved',
                        id: id
                    });
                });

                uploadInfo.id.subscribe(result.id);
                uploadInfo.progress.subscribe(result.progress);
            }
        };

        return result;
    }

    public async save() {
        this.isSaveInProgress = true;

        if (this.request.state == RequestApprovalState.Pending) {
            this.saveBasicInfo();
        }
    }

    private saveBasicInfo() {
        let budgetId = this.budgetId;
        let id = this.requestId;
        let title: string = this.request.title;
        let amount: number = this.request.amount;

        if (this.newRequest) {
            this.saveNewRequest({
                budgetId,
                title,
                amount
            });
        } else if (this.request.state == RequestApprovalState.Pending) {
            this.editExistingRequest({
                id,
                title,
                amount
            });
        }
    }

    private saveNewRequest(payload: NewRequest): void {
        let request =
            this.budgetType == BudgetTypeEnum.TeamBudget
                ? this.requestService.addTeamRequest(payload)
                : this.requestService.addRequest(payload);

        request
            .pipe(concatMap((requestId) => this.uploadInvoices(requestId)))
            .subscribe(
                (_) => {
                    this.dataChangeNotificationService.notify();
                    this.isSaveInProgress = false;
                    this.alertService.alert(
                        new Alert({
                            message: 'Request created successfully',
                            type: AlertType.Success,
                            keepAfterRouteChange: true
                        })
                    );

                    this.router.navigate(['../../'], {
                        relativeTo: this.route
                    });
                },

                (err) => {
                    this.dataChangeNotificationService.notify();
                    this.isSaveInProgress = false;
                    this.alertService.error(
                        'Error while creating request: ' + JSON.stringify(err),
                        'addRequestError'
                    );
                }
            );
    }

    private editExistingRequest(payload: PatchRequest): void {
        let request =
            this.budgetType == BudgetTypeEnum.TeamBudget
                ? this.requestService.updateTeamRequest(payload)
                : this.requestService.updateRequest(payload);

        request.subscribe(
            () => {
                this.dataChangeNotificationService.notify();
                this.isSaveInProgress = false;
                this.alertService.alert(
                    new Alert({
                        message: 'Request updated',
                        type: AlertType.Success,
                        keepAfterRouteChange: true
                    })
                );
                this.dataChangeNotificationService.notify();
            },
            (err) => {
                this.dataChangeNotificationService.notify();
                this.isSaveInProgress = false;
                this.alertService.error(
                    'Error while creating request: ' +
                        JSON.stringify(err.error),
                    'addRequestError'
                );
            }
        );
    }

    public trimTitle(): void {
        this.title = this.title.trim();
    }

    public onHide(): void {
        this.router.navigate(['my-budget']);
    }

    public close(): void {
        this.router.navigate(['my-budget']);
    }
}
