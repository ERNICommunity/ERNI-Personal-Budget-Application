import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { RequestService } from '../../services/request.service';
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
import { ActivatedRoute, Params, Router } from '@angular/router';
import { RequestApprovalState } from '../../model/requestState';

@Component({
    selector: 'app-request-edit',
    templateUrl: 'requestEdit.component.html',
    styleUrls: ['requestEdit.component.css']
})
export class RequestEditComponent implements OnInit {
    httpResponseError: string;
    items: MenuItem[];
    requestId: number;

    popupTitle: string;
    isVisible: boolean;

    budgetId: number;
    budgetType: BudgetTypeEnum;
    newRequest: boolean;
    request: Request;

    title: string;
    amount: number;

    public RequestState = RequestApprovalState; // this is required to be possible to use enum in view

    constructor(private requestService: RequestService,
        private router: Router,
        private route: ActivatedRoute,
        private alertService: AlertService,
        private dataChangeNotificationService: DataChangeNotificationService,
        private busyIndicatorService: BusyIndicatorService) { }

    ngOnInit() {

      this.isVisible = true;

      this.route.params.subscribe((params: Params) => {

        this.budgetId = Number(params['budgetId']);
        this.requestId = Number(params['requestId']);

        if (!isNaN(this.budgetId)) {

          console.log("Creating request");
          this.popupTitle = 'Create new request';
          this.request = this.createNewRequest();
          this.newRequest = true;

        } else if (!isNaN(this.requestId)) {

          console.log("Loading request");
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
      request.date = new Date();
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
              date: new Date(request.date),
              id: request.id,
              state: request.state,
              title: request.title,
              user: request.user,
            } as Request;
            console.log(request);
            console.log(this.request);


          },
          (err) => {
            this.httpResponseError = err.error;
          }
        );
    }

    public save(): void {
      // this.busyIndicatorService.start();

      console.log('Saving');

      if (this.request.state == RequestApprovalState.Pending) {
        console.log("Saving basic info");
        this.saveBasicInfo();
      } else if (this.request.state == RequestApprovalState.Approved) {
        console.log("Saving amount");
        this.updateSpentAmount();
      } else {
        console.log('Else');
      }
    }

    private saveBasicInfo() {
      let budgetId = this.budgetId;
      let id = this.requestId;
      let title: string = this.request.title;
      let amount: number = this.request.amount;
      let date = this.request.date;

        if (this.newRequest) {
            this.saveNewRequest({ budgetId, title, amount, date } as NewRequest);
        } else if (this.request.state == RequestApprovalState.Pending) {
          console.log('Calling editExistingRequest()');
            this.editExistingRequest({ id, title, amount, date } as PatchRequest);
        }

        this.dataChangeNotificationService.notify();
    }

    private saveNewRequest(payload: NewRequest): void {

      console.log('Calling saveNewRequest()');
      console.log(payload);

      let request = this.budgetType == BudgetTypeEnum.TeamBudget
          ? this.requestService.addTeamRequest(payload)
          : this.requestService.addRequest(payload);

      request.subscribe((_) => {
          this.busyIndicatorService.end();
          // this.modal.close();
          this.dataChangeNotificationService.notify();
          this.alertService.alert(new Alert({ message: "Request created successfully", type: AlertType.Success, keepAfterRouteChange: true }));

          this.router.navigate(['my-budget']);
      },
      err => {
          this.busyIndicatorService.end();
          this.alertService.error("Error while creating request: " + JSON.stringify(err.error), "addRequestError");
      });
  }

  private updateSpentAmount() {

  }

  private editExistingRequest(payload: PatchRequest): void {

      let request = this.budgetType == BudgetTypeEnum.TeamBudget
          ? this.requestService.updateTeamRequest(payload)
          : this.requestService.updateRequest(payload);

      request.subscribe(() => {
          this.alertService.alert(new Alert({ message: "Request updated", type: AlertType.Success, keepAfterRouteChange: true }));
          this.dataChangeNotificationService.notify();
          // this.modal.close();
      },
          err => {
              this.alertService.error("Error while creating request: " + JSON.stringify(err.error), "addRequestError");
          });
  }


    public trimTitle() : void {
        this.title = this.title.trim();
    }



    public onHide(): void {
      this.router.navigate(['my-budget']);
    }

    public close(): void {
      this.router.navigate(['my-budget']);
    }
}
