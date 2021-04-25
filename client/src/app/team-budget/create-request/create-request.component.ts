import { Component, OnInit } from "@angular/core";
import { TeamBudgetModel } from "../../model/teamBudget";
import { TeamBudgetService } from "../../services/team-budget.service";
import { PickListModule } from 'primeng/picklist';
import { Router } from "@angular/router";
import { AlertService } from "../../services/alert.service";
import { DataChangeNotificationService } from "../../services/dataChangeNotification.service";

@Component({
  selector: "app-create-request",
  templateUrl: "./create-request.component.html",
  styleUrls: ["./create-request.component.css"],
})
export class CreateRequestComponent implements OnInit {
  isVisible: boolean;
  title: string;
  date: Date;
  amount: number;

  list1: TeamBudgetModel[];

  list2: TeamBudgetModel[];

  maxAmount: number;

  teamBudgets: TeamBudgetModel[];

  constructor(
    private teamBudgetService: TeamBudgetService,
    private alertService: AlertService,
    private dataChangeNotificationService: DataChangeNotificationService,
    private router: Router
  ) {}

  async ngOnInit(): Promise<void> {
    this.isVisible = true;

    this.teamBudgets = await this.teamBudgetService.getAllTeamBudgets(
      new Date().getFullYear()
    );

    this.list1 = this.teamBudgets.filter((_) => !_.employee.isTeamMember);
    this.list2 = this.teamBudgets.filter((_) => _.employee.isTeamMember);

    this.maxAmount = this.getMaxAmount();
  }

  trimTitle(): void {
    this.title = this.title.trim();
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
    return this.list2.reduce<number>((p, c, i, a) => p + c.budgetLeft, 0);
  }

  async save() {
    try {
      await this.teamBudgetService.createTeamRequest({
        employees: this.list2.map((_) => _.employee.id),
        title: this.title,
        amount: this.amount,
        date: this.date,
      });

      this.dataChangeNotificationService.notify();
      this.close();
    } catch (error) {
      this.alertService.error(JSON.stringify(error.error), "addRequestError");
    }
  }

  onHide() {
    this.router.navigate(["team-budget"]);
  }
}