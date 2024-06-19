import { Component, OnInit } from "@angular/core";
import { TeamBudgetModel } from "../../model/teamBudget";
import { TeamBudgetService } from "../../services/team-budget.service";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { AlertService } from "../../services/alert.service";
import { DataChangeNotificationService } from "../../services/dataChangeNotification.service";
import { RequestApprovalState } from "../../model/requestState";

export class RequestViewModel {
  title: string;
  date: Date;
  state?: RequestApprovalState;
  amount: number;
  isReadonly: boolean;
}

@Component({
  selector: "app-create-request",
  templateUrl: "./create-request.component.html",
  styleUrls: ["./create-request.component.css"],
})
export class CreateRequestComponent implements OnInit {
  isVisible: boolean;

  request: RequestViewModel;

  list1: TeamBudgetModel[];

  list2: TeamBudgetModel[];

  popupTitle: string;
  requestId: number;
  newRequest: boolean;
  maxAmount: number;

  teamBudgets: TeamBudgetModel[];

  public RequestState = RequestApprovalState; // this is required to be possible to use enum in view

  constructor(
    private route: ActivatedRoute,
    private teamBudgetService: TeamBudgetService,
    private alertService: AlertService,
    private dataChangeNotificationService: DataChangeNotificationService,
    private router: Router
  ) {}

  async ngOnInit(): Promise<void> {
    this.isVisible = true;

    this.teamBudgets = await this.teamBudgetService.getDefaultTeamBudget(
      new Date().getFullYear()
    );

    this.route.params.subscribe(async (params: Params) => {
      this.requestId = Number(params["requestId"]);
      this.list1 = this.teamBudgets.filter((_) => !_.employee.isTeamMember);
      this.list2 = this.teamBudgets.filter((_) => _.employee.isTeamMember);
      this.maxAmount = this.getMaxAmount();

      if (isNaN(this.requestId)) {
        console.log("Creating request");
        this.popupTitle = "Create new request";
        // this.request = this.createNewRequest();
        this.newRequest = true;

        this.request = new RequestViewModel();
      } else {
        console.log("Loading request");
        this.popupTitle = "Request details";
        await this.loadRequest(this.requestId);
        this.newRequest = false;
      }
    });
  }

  public async loadRequest(requestId: number): Promise<void> {
    this.requestId = requestId;

    const request = await this.teamBudgetService.getSingleTeamRequest(
      requestId
    );

    this.list1 = this.teamBudgets.filter(
      (_) => !request.transactions.find((t) => t.employeeId == _.employee.id)
    );
    this.list2 = this.teamBudgets.filter((_) =>
      request.transactions.find((t) => t.employeeId == _.employee.id)
    );
    this.maxAmount = this.getMaxAmount();

    this.request = {
      amount: request.totalAmount,
      createDate: request.createDate,
      date: new Date(request.createDate),
      id: request.id,
      state: request.state,
      title: request.title,
      isReadonly: true,
    } as RequestViewModel;
    console.log(request);
    console.log(this.request);
  }

  trimTitle(): void {
    this.request.title = this.request.title.trim();
  }
  close() {
    this.router.navigate(["team-budget"]);
  }

  open() {
    this.isVisible = true;
  }

  onAttendeesChanges() {
    this.maxAmount = this.getMaxAmount();
  }

  getMaxAmount() {
    return this.list2.reduce<number>((p, c) => p + c.budgetLeft, 0);
  }

  async save() {
    try {
      if (this.request.state == null) {
        await this.createRequest();
      } else {
        await this.updateRequest();
      }
    } catch (error) {
      this.alertService.error("Save failed", "addRequestError");
    }
  }

  async createRequest(): Promise<void> {
    const id = await this.teamBudgetService.createTeamRequest({
      employees: this.list2.map((_) => _.employee.id),
      title: this.request.title,
      amount: this.request.amount,
      date: this.request.date,
    });

    this.request.state = RequestApprovalState.Approved;

    this.dataChangeNotificationService.notify();

    this.router.navigate([
      "team-budget/" + new Date().getFullYear() + "/request/" + id,
    ]);

    this.alertService.success(
      "Request created. You can upload invoice now.",
      "addRequestError"
    );
  }

  async updateRequest(): Promise<void> {
    await this.teamBudgetService.updateTeamRequest(this.requestId, {
      employees: this.list2.map((_) => _.employee.id),
      title: this.request.title,
      amount: this.request.amount,
      date: this.request.date,
    });

    this.dataChangeNotificationService.notify();

    this.alertService.success("Request updated.", "addRequestError");
  }

  onHide() {
    this.router.navigate(["team-budget"]);
  }
}
