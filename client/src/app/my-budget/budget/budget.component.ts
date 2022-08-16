import { Component, OnInit, Input } from '@angular/core';
import { Budget } from '../../model/budget';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { RequestService } from '../../services/request.service';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { BudgetService } from '../../services/budget.service';
import { RequestApprovalState } from '../../model/requestState';

@Component({
    selector: 'app-budget',
    templateUrl: './budget.component.html',
    styleUrls: ['./budget.component.css']
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
        //private modalService: NgbModal,
        public busyIndicatorService: BusyIndicatorService,
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

    deleteRequest(id: number): void {
        this.requestService.deleteRequest(id).subscribe(() => {
            this.dataChangeNotificationService.notify();
        });
    }

    openDeleteConfirmationModal(content) {
        //this.modalService.open(content, { centered: true, backdrop: 'static' });
    }
}
