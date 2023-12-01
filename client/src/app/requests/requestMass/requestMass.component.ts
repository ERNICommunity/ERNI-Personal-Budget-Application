import { Component, OnInit } from '@angular/core';
import { RequestService } from '../../services/request.service';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { MassRequest } from '../../model/massRequest';
import { AlertService } from '../../services/alert.service';
import { Alert, AlertType } from '../../model/alert.model';
import { ButtonModule } from 'primeng/button';
import { SharedModule } from 'primeng/api';
import { PickListModule } from 'primeng/picklist';
import { InputNumberModule } from 'primeng/inputnumber';
import { NgIf } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { BasicRequestInfoEditorComponent } from '../requestEdit/basic-request-info-editor/basic-request-info-editor.component';

@Component({
    selector: 'app-request-mass',
    templateUrl: './requestMass.component.html',
    styleUrls: ['./requestMass.component.css'],
    standalone: true,
    imports: [BasicRequestInfoEditorComponent, FormsModule, InputTextModule, NgIf, InputNumberModule, PickListModule, SharedModule, ButtonModule]
})
export class RequestMassComponent implements OnInit {
    availableUsers: {
        id: number;
        firstName: string;
        lastName: string;
        budgetLeft: number;
    }[];
    selectedUsers: {
        id: number;
        firstName: string;
        lastName: string;
        budgetLeft: number;
    }[] = [];

    title: string;
    amount: number;
    maximumAmount: number = 0;

    constructor(
        private requestService: RequestService,
        private alertService: AlertService,
        private busyIndicatorService: BusyIndicatorService
    ) {}

    ngOnInit() {
        this.requestService
            .getRemainingBudgets()
            .subscribe((u) => (this.availableUsers = u));
    }

    onAttendeesChanges(): void {
        this.maximumAmount =
            this.selectedUsers.length > 0
                ? this.selectedUsers.reduce(
                      (prev, user) =>
                          prev < user.budgetLeft ? prev : user.budgetLeft,
                      Number.MAX_SAFE_INTEGER
                  )
                : 0;
    }

    save(): void {
        var users = this.selectedUsers;
        this.busyIndicatorService.start();

        let requestData = {
            title: this.title,
            amount: this.amount,
            employees: users.map((_) => _.id)
        } as MassRequest;

        this.requestService
            .addMassRequest(requestData)
            .subscribe(
                () => {
                    this.alertService.alert(
                        new Alert({
                            message: 'Multiple requests created',
                            type: AlertType.Success,
                            keepAfterRouteChange: true
                        })
                    );
                    this.busyIndicatorService.end();
                },
                (err) => {
                    this.alertService.error(
                        'Error while creating request: ' +
                            JSON.stringify(err.error)
                    );
                    this.busyIndicatorService.end();
                }
            )
            .add(() => this.busyIndicatorService.end());
    }
}
