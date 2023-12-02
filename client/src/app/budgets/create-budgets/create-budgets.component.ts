import { Component, Input, inject } from "@angular/core";
import { BudgetTypeEnum } from "../../model/budgetTypeEnum";
import { User } from "../../model/user";
import { AlertService } from "../../services/alert.service";
import { BudgetService } from "../../services/budget.service";
import { DataChangeNotificationService } from "../../services/dataChangeNotification.service";
import { toSignal } from "@angular/core/rxjs-interop";
import { BehaviorSubject, of, switchMap } from "rxjs";
import { SharedModule } from "../../shared/shared.module";

@Component({
  selector: "app-create-budgets",
  templateUrl: "./create-budgets.component.html",
  styleUrls: ["./create-budgets.component.css"],
  standalone: true,
  imports: [SharedModule],
})
export class CreateBudgetsComponent {
  @Input({ required: true })
  public year!: number;

  @Input({ required: true })
  public set budgetType(value: BudgetTypeEnum) {
    this.#budgetType.next(value);
  }

  budgetService = inject(BudgetService);
  alertService = inject(AlertService);
  notificationService = inject(DataChangeNotificationService);

  #budgetType = new BehaviorSubject<BudgetTypeEnum | null>(null);

  budgetTitle?: string;
  amount?: number;

  availableUsers = toSignal(
    this.#budgetType.pipe(
      switchMap((budgetType) =>
        budgetType
          ? this.budgetService.getUsersAvailableForBudgetType(budgetType)
          : of([])
      )
    )
  );
  selectedEmployee?: User | null;

  setBudgetsForYear(): void {
    const task = !this.selectedEmployee
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

    task.subscribe({
      next: () => {
        this.alertService.success("Budget created", "addOtherBudget");
        this.notificationService.notify();

        this.amount = undefined;
        this.budgetTitle = undefined;
        this.selectedEmployee = null;
      },
      error: (err) => {
        this.alertService.error(
          "Error while creating budget: " + JSON.stringify(err.error),
          "addOtherBudget"
        );
      },
    });
  }
}
