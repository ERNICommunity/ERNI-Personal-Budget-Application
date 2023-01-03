import { Component, OnInit, Input } from '@angular/core';
import { Budget } from '../../model/budget';
import { RequestService } from '../../services/request.service';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { BudgetService } from '../../services/budget.service';
import { RequestApprovalState } from '../../model/requestState';
import { ConfirmationService } from 'primeng/api';
import { Request } from '../../model/request/request';

@Component({
    selector: 'app-budget',
    templateUrl: './budget.component.html',
    styleUrls: ['./budget.component.css'],
    providers: [ConfirmationService]
})
export class BudgetComponent implements OnInit {
    @Input() budget: Budget;
    percentageLeft: number;
    requestStateType = RequestApprovalState;
    public currentYear: number;
    budgetTypeName: string;

    constructor(
        private requestService: RequestService,
        private budgetService: BudgetService,
        private confirmationService: ConfirmationService,
        private dataChangeNotificationService: DataChangeNotificationService
    ) {
        this.currentYear = new Date().getFullYear();
    }

    ngOnInit() {
        this.budgetService
            .getBudgetsTypes()
            .subscribe(
                (types) =>
                    (this.budgetTypeName = types.find(
                        (type) => type.id == this.budget.type
                    ).name)
            );
    }

    openDeleteConfirmationModal(request: Request) {
        this.confirmationService.confirm({
            message: `Are you sure you want to delete the request "${request.title}"?`,
            header: 'Delete Confirmation',
            icon: 'pi pi-info-circle',
            accept: () => {
                this.requestService.deleteRequest(request.id).subscribe(() => {
                    this.dataChangeNotificationService.notify();
                });
            }
        });
    }
}
