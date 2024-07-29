import { Component, input, computed, inject } from "@angular/core";
import { RequestService } from "../../services/request.service";
import { DataChangeNotificationService } from "../../services/dataChangeNotification.service";
import { BudgetService } from "../../services/budget.service";
import { RequestApprovalState } from "../../model/requestState";
import { ConfirmationService } from "primeng/api";
import { Request } from "../../model/request/request";
import { BudgetTypeEnum } from "../../model/budgetTypeEnum";
import { UserState } from "../../model/userState";
import { toSignal } from "@angular/core/rxjs-interop";

export interface UserModel {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  superior: UserModel;
  state: UserState;
}

export interface BudgetModel {
  id: number;
  year: number;
  amount: number;
  amountLeft: number;
  isEditable: boolean;
  user: UserModel;
  title: string;
  type: BudgetTypeEnum;
  requests: Request[];
}

@Component({
  selector: "app-budget",
  templateUrl: "./budget.component.html",
  styleUrls: ["./budget.component.css"],
  providers: [ConfirmationService],
})
export class BudgetComponent {
  requestService = inject(RequestService);
  budgetService = inject(BudgetService);
  confirmationService = inject(ConfirmationService);
  dataChangeNotificationService = inject(DataChangeNotificationService);

  budget = input.required<BudgetModel>();

  budgetTypes = toSignal(this.budgetService.getBudgetsTypes(), { initialValue: [] });
  budgetTypeName = computed(
    () => this.budgetTypes().find((type) => type.id == this.budget().type)?.name
  );
  budgetAmountPercentage = computed(() => (100 * this.budget().amountLeft) / this.budget().amount);
  budgetAmountProgressColor = computed(() => {
    if (!this.budget().isEditable) {
      return '#828282';
    }
    if (this.budgetAmountPercentage() < 25) {
      return '#D03E35';
    }
    if (this.budgetAmountPercentage() < 50) {
      return '#D0A335';
    }
    return '#55AB55';
  });
  budgetIcon = computed(() => {
    const budgetTypeName = this.budgetTypeName();

    if (!budgetTypeName) {
      return null;
    }

    if (this.budget().type === BudgetTypeEnum.PersonalBudget) {
      return '💰';
    }
    if (this.budget().type === BudgetTypeEnum.RecreationBudget) {
      return '🏖️';
    }
    if (this.budget().type === BudgetTypeEnum.CommunityBudget) {
      return '️🚴';
    }
    return null;
  });

  requestStateType = RequestApprovalState;

  openDeleteConfirmationModal(request: Request) {
    this.confirmationService.confirm({
      message: `Are you sure you want to delete the request "${request.title}"?`,
      header: "Delete Confirmation",
      icon: "pi pi-info-circle",
      accept: () => {
        this.requestService.deleteRequest(request.id).subscribe(() => {
          this.dataChangeNotificationService.notify();
        });
      },
    });
  }
}
