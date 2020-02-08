import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Request } from '../../model/request/request';
import { RequestService } from '../../services/request.service';
import { FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { BudgetTypeEnum } from '../../model/budgetTypeEnum';

@Component({
    selector: 'app-request-detail',
    templateUrl: 'requestDetail.component.html',
    styleUrls: ['requestDetail.component.css']
})
export class RequestDetailComponent implements OnInit {
    request: Request;
    selectedDate: Date;
    requestForm: FormGroup;
    httpResponseError: string;
    requestType: BudgetTypeEnum;

    constructor(private requestService: RequestService,
        private location: Location,
        public modal: NgbActiveModal) {
    }

    ngOnInit() { }

    public getRequest(id: number): void {
        if (this.requestType == BudgetTypeEnum.TeamBudget) {
            this.requestService.getTeamRequest(id)
                .subscribe(request => {
                    this.request = request;
                    this.selectedDate = new Date(request.date);
                }, err => {
                    this.httpResponseError = err.error
                });
        } else {
            this.requestService.getRequest(id)
                .subscribe(request => {
                    this.request = request;
                    this.selectedDate = new Date(request.date);
                }, err => {
                    this.httpResponseError = err.error
                });
        }
    }

    goBack(): void {
        this.location.back();
    }
}
