import { Component } from '@angular/core';
import { Location } from '@angular/common';
import { RequestService } from '../../services/request.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { PatchRequest } from '../../model/PatchRequest';
import { NgbDate, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertService } from '../../services/alert.service';
import { Alert, AlertType } from '../../model/alert.model';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { BudgetTypeEnum } from '../../model/budgetTypeEnum';
import { InvoicedAmount } from '../../model/invoicedAmount';

@Component({
    selector: 'app-request-edit',
    templateUrl: 'requestEdit.component.html',
    styleUrls: ['requestEdit.component.css']
})
export class RequestEditComponent {
    requestForm: FormGroup;
    httpResponseError: string;
    dirty: boolean;

    requestId: number;
    budgetType: BudgetTypeEnum;

    constructor(private requestService: RequestService,
        public modal: NgbActiveModal,
        private location: Location,
        private fb: FormBuilder,
        private alertService: AlertService,
        private dataChangeNotificationService: DataChangeNotificationService) {
        this.createForm();
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
                this.requestId = id;

                var date = new Date(request.date);

                var ngbDate = new NgbDate(date.getFullYear(), date.getMonth() + 1, date.getDate());

                this.requestForm.setValue({
                    title: request.title,
                    amount: request.amount,
                    date: ngbDate
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
    }
}
