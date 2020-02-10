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

@Component({
    selector: 'app-request-add',
    templateUrl: './requestAdd.component.html',
    styleUrls: ['./requestAdd.component.css']
})
export class RequestAddComponent implements OnInit {
    @Input() budget: Budget;
    httpResponseError: string;
    requestForm: FormGroup;

    budgetId: number;

  constructor(
    public modal: NgbActiveModal,
    private requestService: RequestService,
    private fb: FormBuilder,
    private busyIndicatorService: BusyIndicatorService,
    private alertService: AlertService,
    private dataChangeNotificationService: DataChangeNotificationService) {
    this.createForm();
  }

    ngOnInit() {
    }

    createForm() {
        this.requestForm = this.fb.group({
            title: ['', Validators.required],
            amount: ['', Validators.required],
            date: ['', Validators.required],
        });
    }

    save(): void {
        var budgetId = this.budgetId;
        var title = this.requestForm.get("title").value;
        var amount = this.requestForm.get("amount").value;
        var ngbDate = this.requestForm.get("date").value;

        var date = new Date(ngbDate.year, ngbDate.month - 1, ngbDate.day);

        this.busyIndicatorService.start();

    this.requestService.addRequest({ budgetId, title, amount, date } as NewRequest)
      .subscribe(() => {
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