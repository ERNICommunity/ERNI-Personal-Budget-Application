import { Component, Input } from '@angular/core';
import { Budget } from '../../model/budget';
import { Request } from '../../model/request/request'
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { RequestService } from '../../services/request.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { RequestState } from '../../model/requestState';
import { RequestFilter } from '../../requests/requestFilter';

@Component({
    selector: 'app-budget',
    templateUrl: './budget.component.html',
    styleUrls: ['./budget.component.css']
})
export class BudgetComponent {
    @Input() budget: Budget;
    requests: Request[];
    percentageLeft: number;
    requestStateType = RequestFilter;

    constructor(private requestService: RequestService,
        private modalService: NgbModal,
        public busyIndicatorService: BusyIndicatorService) { }

    getRequests(): void {
        this.busyIndicatorService.start();
        this.requestService.getRequests(this.budget.id)
            .subscribe(requests => {
                this.requests = requests;
                this.getCurrentAmount(requests);
                this.busyIndicatorService.end();
            });
    }

    deleteRequest(id: number): void {
        this.requestService.deleteRequest(id).subscribe(() => {
            this.requests = this.requests.filter(req => req.id !== id),
                this.getCurrentAmount(this.requests)
        });
    }

    openDeleteConfirmationModal(content) {
        this.modalService.open(content, { centered: true, backdrop: 'static' });
    }

    getCurrentAmount(requests: Request[]): void {

        requests.forEach((req) => {
            if (req.state != RequestState.Rejected) {
            }
        });
    }

}
