import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { BudgetTypeEnum } from '../../model/budgetTypeEnum';
import { User } from '../../model/user';
import { AlertService } from '../../services/alert.service';
import { BudgetService } from '../../services/budget.service';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';

@Component({
  selector: 'app-create-budgets',
  templateUrl: './create-budgets.component.html',
  styleUrls: ['./create-budgets.component.css'],
})
export class CreateBudgetsComponent implements OnInit, OnChanges {
  @Input({ required: true })
  public year!: number;

  @Input({ required: true })
  public budgetType!: BudgetTypeEnum;

  budgetTitle = '';
  amount: number | undefined;

  availableUsers: User[] = [];
  selectedEmployee: User | null = null;

  constructor(
    private budgetService: BudgetService,
    private alertService: AlertService,
    private notificationService: DataChangeNotificationService,
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['year'] !== undefined || changes['budgetType'] !== undefined) {
      this.ngOnInit();
    }
  }

  ngOnInit(): void {
    this.budgetService
      .getUsersAvailableForBudgetType(this.budgetType)
      .subscribe((users) => (this.availableUsers = users));
  }

  setBudgetsForYear(): void {
    if (this.amount === undefined || this.budgetTitle?.length === 0) {
      return;
    }

    const task = !this.selectedEmployee
      ? this.budgetService.createBudgetsForAllActiveUsers(this.budgetTitle, this.amount, this.budgetType)
      : this.budgetService.createBudget(this.budgetTitle, this.amount, this.selectedEmployee.id, this.budgetType);

    task.subscribe({
      next: () => {
        this.alertService.success('Budget created', 'addOtherBudget');
        this.notificationService.notify();

        this.amount = undefined;
        this.budgetTitle = '';
        this.selectedEmployee = null;
      },
      error: (err) => {
        this.alertService.error('Error while creating budget: ' + JSON.stringify(err.error), 'addOtherBudget');
      },
    });
  }
}
