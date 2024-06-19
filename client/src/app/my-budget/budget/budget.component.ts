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

  budgetTypes = toSignal(this.budgetService.getBudgetsTypes());

  requestStateType = RequestApprovalState;
  currentYear = new Date().getFullYear();

  budgetTypeName = computed(
    () =>
      this.budgetTypes()?.find((type) => type.id == this.budget().type)?.name ??
      ""
  );

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
