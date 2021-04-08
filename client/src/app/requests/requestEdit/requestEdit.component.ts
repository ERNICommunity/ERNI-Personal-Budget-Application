import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { RequestService } from '../../services/request.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbDate, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertService } from '../../services/alert.service';
import { Alert, AlertType } from '../../model/alert.model';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { BudgetTypeEnum } from '../../model/budgetTypeEnum';
import { InvoicedAmount } from '../../model/invoicedAmount';
import { MenuItem } from 'primeng/api';
import { InvoiceImageService } from '../../services/invoice-image.service';
import { Request } from '../../model/request/request';
import { PatchRequest } from '../../model/PatchRequest';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { NewRequest } from '../../model/newRequest';

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
    requestForm: FormGroup;
    httpResponseError: string;
    dirty: boolean;
    items: MenuItem[];
    requestId: number;
    budgetType: BudgetTypeEnum; 
    events: any[];
    requestState: RequestState;
    activeState: RequestState;
    activeStateIndex: number;
    RequestState = RequestState;
    @ViewChild('downloadLink',{static : false}) downloadLink: ElementRef;
    request: Request;
    selectedDate : Date;
    images: [number, string][];

    title: string;
    amount: number;
    date: any;
    budgetId: number;

    constructor(private requestService: RequestService,
        public modal: NgbActiveModal,
        private location: Location,
        private fb: FormBuilder,
        private alertService: AlertService,
        private dataChangeNotificationService: DataChangeNotificationService,
        private invoiceImageService: InvoiceImageService,
        private busyIndicatorService: BusyIndicatorService) {
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
        this.switchState(RequestState.Request);
        this.budgetId = budgetId;
        this.budgetType = budgetType;
    }

    public openPending(requestId: number): void {
        this.switchState(RequestState.Pending);
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

    public openClosed(requestId: number): void {
        this.switchState(RequestState.Closed);
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
 

    private createForm(): void {
        this.requestForm = this.fb.group({
            title: ['', Validators.required],
            amount: ['', Validators.required],
            date: ['', Validators.required]
        });
    }

    switchState(newState: RequestState): void {
        this.activeState = newState;
        this.activeStateIndex = newState;
    }

    public save(): void {
        console.log(this.date);
        if (this.activeState == RequestState.Request) {
            this.createNewRequest();
        }

        if (this.activeState == RequestState.Pending) {
            this.editExistingRequest();
        }
    }

    private createNewRequest(): void {
        this.busyIndicatorService.start();
        console.log('creating')
        let budgetId = this.budgetId;
        let title = this.title;
        let amount = this.amount;

        let date = new Date(this.date.year, this.date.month, this.date.day);

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
    
    trimTitle() : void {
        this.title = this.title.trim();
    }

    setDirty(): void {
        this.dirty = true;
    }

    isDirty(): boolean {
        return this.dirty;
    }

    goBack(): void {
        this.location.back();
    }

    private editExistingRequest(): void {
        let title = this.requestForm.get("title").value;
        let amount = this.requestForm.get("amount").value;
        let ngbDate = this.requestForm.get("date").value;
        let date = new Date(ngbDate.year, ngbDate.month - 1, ngbDate.day);
        let id = this.requestId;
        // SAVE

        let requestData = { id, title, amount, date } as PatchRequest;
        let request = this.budgetType == BudgetTypeEnum.TeamBudget
            ? this.requestService.updateTeamRequest(requestData)
            : this.requestService.updateInvoicedAmount(1, new InvoicedAmount({ amount: 7.77 }));

        request.subscribe(() => {
            this.alertService.alert(new Alert({ message: "Request updated", type: AlertType.Success, keepAfterRouteChange: true }));
            this.dataChangeNotificationService.notify();
            this.modal.close();
        },
            err => {
                this.alertService.error("Error while creating request: " + JSON.stringify(err.error), "addRequestError");
            });
    }  download(imageId: number, imageName: string) {
    this.invoiceImageService.getInvoiceImage(imageId).subscribe(blob => { this.processBlob(blob, imageName); })
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