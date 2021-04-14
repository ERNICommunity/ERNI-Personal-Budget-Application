import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { RequestService } from '../../services/request.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbDate, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertService } from '../../services/alert.service';
import { Alert, AlertType } from '../../model/alert.model';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { BudgetTypeEnum } from '../../model/budgetTypeEnum';
import { MenuItem } from 'primeng/api';
import { InvoiceImageService } from '../../services/invoice-image.service';
import { Request } from '../../model/request/request';
import { PatchRequest } from '../../model/PatchRequest';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { NewRequest } from '../../model/newRequest';
import { AuthenticationService } from '../../services/authentication.service';

export enum RequestState {
    Request,
    Pending,
    Invoice,
    Closed
}

@Component({
    selector: 'app-request-edit',
    templateUrl: 'requestEdit.component.html',
    styleUrls: ['requestEdit.component.css']
})
export class RequestEditComponent implements OnInit {
    @ViewChild('downloadLink',{static : false}) downloadLink: ElementRef;
    requestForm: FormGroup;
    httpResponseError: string;
    items: MenuItem[];
    requestId: number;
    
    budgetId: number;
    budgetType: BudgetTypeEnum;    
    request: Request;
    selectedDate : Date;
    images: [number, string][];

    title: string;
    amount: number;
    date: any;

    public RequestState = RequestState; // this is required to be possible to use enum in view
    public requestState: RequestState;

    constructor(private requestService: RequestService,
        public modal: NgbActiveModal,
        private fb: FormBuilder,
        private alertService: AlertService,
        private dataChangeNotificationService: DataChangeNotificationService,
        private invoiceImageService: InvoiceImageService,
        private busyIndicatorService: BusyIndicatorService,
        private authService: AuthenticationService,
        private dataNotificationService: DataChangeNotificationService) {
        this.createForm();
    }

    ngOnInit() {
        this.items = [
            { label: RequestState[RequestState.Request] },
            { label: RequestState[RequestState.Pending] },
            { label: RequestState[RequestState.Invoice] },
            { label: RequestState[RequestState.Closed] }
        ];
    }

    public openRequest(budgetId: number, budgetType: number): void {
        this.requestState = RequestState.Request;
        this.budgetId = budgetId;
        this.budgetType = budgetType;
    }

    public openPending(requestId: number): void {
        this.requestState = RequestState.Pending;
        this.requestService
            .getRequest(requestId)
            .subscribe(request => {
                this.request = request;
                this.requestId = requestId;
                this.selectedDate = new Date(request.date);

                var ngbDate = new NgbDate(this.selectedDate.getFullYear(), this.selectedDate.getMonth() + 1, this.selectedDate.getDate());

                this.requestForm.setValue({
                    title: request.title,
                    amount: request.amount,
                    date: ngbDate
                });                
                this.invoiceImageService.getInvoiceImages(requestId).subscribe(names => {
                    this.images = names;
                });
            }, err => {
                this.httpResponseError = err.error
            });
    }

    public openInvoice(requestId: number): void {
        console.log('invoice')
        this.requestState = RequestState.Invoice;
        this.requestId = requestId;
        this.requestService
        .getRequest(requestId)
        .subscribe(request => { 
            this.request = request;
            this.selectedDate = new Date(request.date);
            this.invoiceImageService
                .getInvoiceImages(requestId)
                .subscribe(names => {
                    this.images = names;
                });
        },
        err => {
            this.httpResponseError = err.error
        });
    }

    public openClosed(requestId: number): void {
        this.requestState = RequestState.Closed;
        this.requestService
            .getRequest(requestId)
            .subscribe(request => { 
                this.request = request;
                this.selectedDate = new Date(request.date);
                this.invoiceImageService
                    .getInvoiceImages(requestId)
                    .subscribe(names => {
                        this.images = names;
                    });
            },
            err => {
                this.httpResponseError = err.error
            });
    }

    public save(): void {
        console.log(this.date);
        if (this.requestState == RequestState.Request) {
            this.createNewRequest();
        }

        if (this.requestState == RequestState.Pending) {
            this.editExistingRequest();
        }        

        this.dataChangeNotificationService.notify();
    }
        
    public download(imageId: number, imageName: string): void {
        this.invoiceImageService.getInvoiceImage(imageId).subscribe(blob => { this.processBlob(blob, imageName); })
    }

    public trimTitle() : void {
        this.title = this.title.trim();
    }

    public canChangeState(): boolean {
        return (this.requestState == RequestState.Pending || this.requestState == RequestState.Invoice) &&
            (this.authService.userInfo.isAdmin || this.authService.userInfo.isFinance);
    }

    public approve(): void {
        if (this.requestState == RequestState.Pending) {
            this.requestState = RequestState.Invoice;
            this.requestService.approveRequest(this.requestId)
                .subscribe(_ => {
                    this.dataChangeNotificationService.notify();
                },
                err => {
                    this.httpResponseError = err.error
                });
            return;
        }

        if (this.requestState == RequestState.Invoice) {
            this.requestState = RequestState.Closed;
            this.requestService.completeRequest(this.requestId)
                .subscribe(_ => {
                    this.dataChangeNotificationService.notify();
                },
                err => {
                    this.httpResponseError = err.error
                });
            return;
        }
    }

    public reject(): void {
        this.requestState = RequestState.Closed;
        this.requestService.rejectRequest(this.requestId)
            .subscribe(_ => {
                this.dataChangeNotificationService.notify();
            },
            err => {
                this.httpResponseError = err.error
            });
    }

    private createForm(): void {
        this.requestForm = this.fb.group({
            title: ['', Validators.required],
            amount: ['', Validators.required],
            date: ['', Validators.required]
        });
    }

    private createNewRequest(): void {
        this.busyIndicatorService.start();
        
        let budgetId = this.budgetId;
        let title: string = this.requestForm.get("title").value;
        let amount: number = this.requestForm.get("amount").value;
        let ngbDate = this.requestForm.get("date").value;
        let date = new Date(ngbDate.year, ngbDate.month - 1, ngbDate.day);

        let requestData = { budgetId, title, amount, date } as NewRequest;
        let request = this.budgetType == BudgetTypeEnum.TeamBudget
            ? this.requestService.addTeamRequest(requestData)
            : this.requestService.addRequest(requestData);

        request.subscribe(() => {
            this.busyIndicatorService.end();
            this.modal.close();
            this.dataChangeNotificationService.notify();
            this.alertService.alert(new Alert({ message: "Request created successfully", type: AlertType.Success, keepAfterRouteChange: true }));
        },
        err => {
            this.busyIndicatorService.end();
            this.alertService.error("Error while creating request: " + JSON.stringify(err.error), "addRequestError");
        });
    }

    private editExistingRequest(): void {
        let title = this.requestForm.get("title").value;
        let amount = this.requestForm.get("amount").value;
        let ngbDate = this.requestForm.get("date").value;
        let date = new Date(ngbDate.year, ngbDate.month - 1, ngbDate.day);
        let id = this.requestId;

        let requestData = { id, title, amount, date } as PatchRequest;
        let request = this.budgetType == BudgetTypeEnum.TeamBudget
            ? this.requestService.updateTeamRequest(requestData)
            : this.requestService.updateRequest(requestData);

        request.subscribe(() => {
            this.alertService.alert(new Alert({ message: "Request updated", type: AlertType.Success, keepAfterRouteChange: true }));
            this.dataChangeNotificationService.notify();
            this.modal.close();
        },
            err => {
                this.alertService.error("Error while creating request: " + JSON.stringify(err.error), "addRequestError");
            });
    }

    private processBlob(blob: Blob, name: string): void {
        let fileObject = new File([blob], name);
        let url = window.URL.createObjectURL(fileObject);
        let link = this.downloadLink.nativeElement;
        link.setAttribute('download', name);
        link.href = url;
        link.click();
        window.URL.revokeObjectURL(url);
    }
}