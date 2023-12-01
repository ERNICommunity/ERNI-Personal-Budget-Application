import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
} from "@angular/core";
import { BudgetType } from "../../model/budgetType";
import { BudgetTypeEnum } from "../../model/budgetTypeEnum";
import { User } from "../../model/user";
import { AlertService } from "../../services/alert.service";
import { BudgetService } from "../../services/budget.service";
import { DataChangeNotificationService } from "../../services/dataChangeNotification.service";
import { ButtonModule } from "primeng/button";
import { SharedModule } from "primeng/api";
import { DropdownModule } from "primeng/dropdown";
import { InputNumberModule } from "primeng/inputnumber";
import { NgIf } from "@angular/common";
import { InputTextModule } from "primeng/inputtext";
import { FormsModule } from "@angular/forms";
import { AlertComponent } from "../../shared/alert/alert.component";

@Component({
  selector: "app-create-budgets",
  templateUrl: "./create-budgets.component.html",
  styleUrls: ["./create-budgets.component.css"],
  standalone: true,
  imports: [
    AlertComponent,
    FormsModule,
    InputTextModule,
    NgIf,
    InputNumberModule,
    DropdownModule,
    SharedModule,
    ButtonModule,
  ],
})
export class CreateBudgetsComponent implements OnInit, OnChanges {
  @Input({ required: true })
  public year!: Number;

  @Input({ required: true })
  public budgetType!: BudgetTypeEnum;

  budgetTitle?: string;
  amount?: number;

  availableUsers?: User[];
  selectedEmployee?: User | null;

  constructor(
    private budgetService: BudgetService,
    private alertService: AlertService,
    private notificationService: DataChangeNotificationService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes["year"] !== undefined || changes["budgetType"] !== undefined) {
      this.ngOnInit();
    }
  }

  ngOnInit(): void {
    this.budgetService
      .getUsersAvailableForBudgetType(this.budgetType)
      .subscribe((users) => (this.availableUsers = users));
  }

  setBudgetsForYear(): void {
    var task = !this.selectedEmployee
      ? this.budgetService.createBudgetsForAllActiveUsers(
          this.budgetTitle!,
          this.amount!,
          this.budgetType
        )
      : this.budgetService.createBudget(
          this.budgetTitle!,
          this.amount!,
          this.selectedEmployee.id,
          this.budgetType
        );

    task.subscribe(
      () => {
        this.alertService.success("Budget created", "addOtherBudget");
        this.notificationService.notify();

        this.amount = undefined;
        this.budgetTitle = undefined;
        this.selectedEmployee = null;
      },
      (err) => {
        this.alertService.error(
          "Error while creating budget: " + JSON.stringify(err.error),
          "addOtherBudget"
        );
      }
    );
  }
}
