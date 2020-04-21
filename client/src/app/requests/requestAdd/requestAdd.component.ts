import { Component, OnInit, Input } from '@angular/core';
import { RequestService } from '../../services/request.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { Budget } from '../../model/budget';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertService } from '../../services/alert.service';
import { NewRequest } from '../../model/newRequest';
import { Alert, AlertType } from '../../model/alert.model';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { BudgetTypeEnum } from '../../model/budgetTypeEnum';

@Component({
    selector: 'app-request-add',
    templateUrl: './requestAdd.component.html',
    styleUrls: ['./requestAdd.component.css']
})
export class RequestAddComponent implements OnInit {
    @Input() budget: Budget;
    httpResponseError: string;

    title: string;
    amount: number;
    date: any;

    budgetId: number;
    budgetType: BudgetTypeEnum;

    constructor(
        public modal: NgbActiveModal,
        private requestService: RequestService,
        private busyIndicatorService: BusyIndicatorService,
        private alertService: AlertService,
        private dataChangeNotificationService: DataChangeNotificationService) {
    }

    ngOnInit() { }

    save(): void {
        this.busyIndicatorService.start();

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
}