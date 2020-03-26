import { Component, OnInit, Input } from '@angular/core';
import { Budget } from '../../model/budget';
import { Request } from '../../model/request/request'
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { RequestService } from '../../services/request.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { RequestFilter } from '../../requests/requestFilter';
import { RequestAddComponent } from '../../requests/requestAdd/requestAdd.component';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { BudgetService } from '../../services/budget.service';

@Component({
  selector: 'app-budget',
  templateUrl: './budget.component.html',
  styleUrls: ['./budget.component.css']
})
export class BudgetComponent implements OnInit {
  @Input() budget: Budget;
  requests: Request[];
  percentageLeft: number;
  requestStateType = RequestFilter;
  public currentYear: number;
  budgetTypeName :  string;

  constructor(private requestService: RequestService,
    private budgetService : BudgetService,
    private modalService: NgbModal,
    public busyIndicatorService: BusyIndicatorService,
    private dataChangeNotificationService: DataChangeNotificationService) {
    this.currentYear = (new Date()).getFullYear();
  }

  ngOnInit() {
    this.budgetService.getBudgetsTypes()
    .subscribe(types => this.budgetTypeName = types
      .find(type => type.id == this.budget.type ).name);}

  createRequest() {
    const modalRef = this.modalService.open(RequestAddComponent);
  }

  deleteRequest(id: number): void {
    this.requestService.deleteRequest(id).subscribe(() => {
      this.dataChangeNotificationService.notify();
    });
  }

  openDeleteConfirmationModal(content) {
    this.modalService.open(content, { centered: true, backdrop: 'static' });
  }
}