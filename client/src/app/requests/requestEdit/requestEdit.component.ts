import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
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
import { Image, NewImage } from '../../shared/file-list/file-list.component';
import { concatMap, delay, tap } from 'rxjs/operators';
import { forkJoin, Observable, of, Subject, zip } from 'rxjs';
import { InvoiceImage } from '../../model/InvoiceImage';

@Component({
    selector: 'app-request-edit',
    templateUrl: 'requestEdit.component.html',
    styleUrls: ['requestEdit.component.css']
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
    public images: (Image | NewImage)[];

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
                console.log('Creating request');
                this.popupTitle = 'Create new request';
                this.request = this.createNewRequest();
                this.newRequest = true;
                this.images = [];
            } else if (!isNaN(this.requestId)) {
                console.log('Loading request');
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

        this.invoiceImageService
            .getInvoiceImages(this.requestId)
            .subscribe((names) => (this.images = names));
    }

    public onNewImageAdded(files: FileList) {
        for (var i = 0; i < files.length; i++) {
            const selectedFile = files[i];

            const im = new NewImage();
            im.name = selectedFile.name;
            im.file = selectedFile;
            this.images.push(im);
        }
    }

    private uploadImages(requestId: number): Observable<any> {
        console.log('Uploading images');
        console.log(this.images);
        const images = this.images.filter((_) => _ instanceof NewImage);
        console.log(images);
        const uploads = images.map((_) =>
            this.uploadImage(requestId, _ as NewImage)
        );

        return forkJoin(uploads);
    }

    private uploadImage(
        requestId: number,
        image: NewImage
    ): Observable<number> {
        const progress = new Subject<number>();

        const fileReader = new FileReader();
        fileReader.readAsDataURL(image.file);
        fileReader.onload = () => {
            if (fileReader.result) {
                const payload: InvoiceImage = {
                    requestId: requestId,
                    data: fileReader.result
                        .toString()
                        .replace('data:', '')
                        .replace(/^.+,/, ''),
                    filename: image.name,
                    mimeType: image.file.type
                };

                image.progress = 0;

                return this.invoiceImageService
                    .addInvoiceImage(payload)
                    .pipe(
                        tap((progress) => {
                            image.progress = progress;
                        })
                    )
                    .subscribe(progress);
            }
        };

        return progress;
    }

    public async save() {
        //this.busyIndicatorService.start();
        this.isSaveInProgress = true;

        if (this.request.state == RequestApprovalState.Pending) {
            console.log('Saving basic info');
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
            console.log('Calling editExistingRequest()');
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
            .pipe(
                delay(4000),
                concatMap((requestId) => this.uploadImages(requestId))
            )
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

                    this.router.navigate(['my-budget']);
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

    private editExistingRequest(payload: PatchRequest): void {
        let request =
            this.budgetType == BudgetTypeEnum.TeamBudget
                ? this.requestService.updateTeamRequest(payload)
                : this.requestService.updateRequest(payload);

        forkJoin([request, this.uploadImages(payload.id)]).subscribe(
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
