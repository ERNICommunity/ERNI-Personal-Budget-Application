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
    budgetType: BudgetTypeEnum; events: any[];

    requestState: RequestState;
    activeState: RequestState;
    activeStateIndex: number;

    RequestState = RequestState;

    @ViewChild('downloadLink',{static : false}) downloadLink: ElementRef;
    request: Request;
    selectedDate : Date;
    images: [number, string][];



    constructor(private requestService: RequestService,
        public modal: NgbActiveModal,
        private location: Location,
        private fb: FormBuilder,
        private alertService: AlertService,
        private dataChangeNotificationService: DataChangeNotificationService,
        private invoiceImageService: InvoiceImageService) {
        this.createForm();
    }

    ngOnInit() {
        this.items = [
            { label: RequestState[RequestState.Request] },
            { label: RequestState[RequestState.Pending] },
            { label: RequestState[RequestState.Invoice] },
            { label: RequestState[RequestState.Closed] }
        ];
        this.switchState(RequestState.Pending);
    }

    switchState(newState: RequestState): void {
        this.activeState = newState;
        this.activeStateIndex = newState;
    }
 

    createForm() {
        this.requestForm = this.fb.group({
            title: ['', Validators.required],
            amount: ['', Validators.required],
            date: ['', Validators.required]
        });
    }

    public showRequest(id: number): void {
        this.requestService.getRequest(id)
            .subscribe(request => {
                this.request = request;
                this.requestId = id;
                this.selectedDate = new Date(request.date);

                var ngbDate = new NgbDate(this.selectedDate.getFullYear(), this.selectedDate.getMonth() + 1, this.selectedDate.getDate());

                this.requestForm.setValue({
                    title: request.title,
                    amount: request.amount,
                    date: ngbDate
                });                
                this.invoiceImageService.getInvoiceImages(id).subscribe(names => {
                    this.images = names;
                });
            }, err => {
                this.httpResponseError = err.error
            });    
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

    save(): void {
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

  processBlob(blob: Blob, name: string) {
    let fileObject = new File([blob], name);
    let url = window.URL.createObjectURL(fileObject);
    let link = this.downloadLink.nativeElement;
    link.setAttribute('download', name);
    link.href = url;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}