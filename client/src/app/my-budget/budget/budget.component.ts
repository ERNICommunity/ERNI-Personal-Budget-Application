import { Component, OnInit, Input } from "@angular/core";
import { Budget } from "../../model/budget";
import { RequestService } from "../../services/request.service";
import { DataChangeNotificationService } from "../../services/dataChangeNotification.service";
import { BudgetService } from "../../services/budget.service";
import { RequestApprovalState } from "../../model/requestState";
import { ConfirmationService } from "primeng/api";
import { Request } from "../../model/request/request";
import { DecimalPipe, DatePipe } from "@angular/common";
import { SharedModule } from "../../shared/shared.module";

@Component({
  selector: "app-budget",
  templateUrl: "./budget.component.html",
  styleUrls: ["./budget.component.css"],
  providers: [ConfirmationService],
  standalone: true,
  imports: [SharedModule, DecimalPipe, DatePipe],
})
export class BudgetComponent {
  @Input({ required: true }) budget!: Budget & { typeName: string };

  requestStateType = RequestApprovalState;

  constructor(
    private requestService: RequestService,
    private confirmationService: ConfirmationService,
    private dataChangeNotificationService: DataChangeNotificationService
  ) {}

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
