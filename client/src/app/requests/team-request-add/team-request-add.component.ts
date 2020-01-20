import { Component, OnInit, Input } from '@angular/core';
import { NewRequest } from '../../model/newRequest';
import { Budget } from '../../model/budget';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { RequestService } from '../../services/request.service';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { AlertService } from '../../services/alert.service';
import { Alert, AlertType } from '../../model/alert.model';
import { TeamBudget } from '../../model/request/team-budget';

@Component({
    selector: 'app-team-request-add',
    templateUrl: './team-request-add.component.html',
    styleUrls: ['./team-request-add.component.css']
})
export class TeamRequestAddComponent implements OnInit {
    httpResponseError: string;
    requestForm: FormGroup;

    constructor(
        public modal: NgbActiveModal,
        private requestService: RequestService,
        private fb: FormBuilder,
        private busyIndicatorService: BusyIndicatorService,
        private alertService: AlertService) {
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
        var title = this.requestForm.get("title").value;
        var amount = this.requestForm.get("amount").value;
        var ngbDate = this.requestForm.get("date").value;

        var date = new Date(ngbDate.year, ngbDate.month - 1, ngbDate.day);

        this.busyIndicatorService.start();

        this.requestService.addTeamRequest({ title, amount, date } as NewRequest)
            .subscribe(() => {
                this.busyIndicatorService.end();
                this.modal.close();
                this.alertService.alert(new Alert({ message: "Request created successfully", type: AlertType.Success, keepAfterRouteChange: true }));
            },
                err => {
                    this.busyIndicatorService.end();
                    this.alertService.error("Error while creating request: " + JSON.stringify(err.error), "addRequestError");
                });
    }
}
