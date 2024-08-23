import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RequestService } from '../../services/request.service';
import { DataChangeNotificationService } from '../../services/dataChangeNotification.service';
import { BudgetService } from '../../services/budget.service';
import { RequestApprovalState } from '../../model/requestState';
import { ConfirmationService } from 'primeng/api';
import { Request } from '../../model/request/request';
import { BudgetTypeEnum } from '../../model/budgetTypeEnum';
import { UserState } from '../../model/userState';
import { toSignal } from '@angular/core/rxjs-interop';

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

const requestStateChipColor: Record<RequestApprovalState, string | undefined> = {
  [RequestApprovalState.Approved]: 'bg-green-100',
  [RequestApprovalState.Pending]: undefined,
  [RequestApprovalState.Rejected]: 'bg-red-100',
};

const budgetTypeToIcon: Record<BudgetTypeEnum, string | null> = {
  [BudgetTypeEnum.PersonalBudget]: '💵',
  [BudgetTypeEnum.RecreationBudget]: '🏖️',
  [BudgetTypeEnum.CommunityBudget]: '🚴',
  [BudgetTypeEnum.TeamBudget]: null,
};

const budgetTypeToInfoLink: Record<BudgetTypeEnum, string | null> = {
  [BudgetTypeEnum.PersonalBudget]: 'https://erniegh.sharepoint.com/sites/people/benefit/ESK/Pages/Personal-budget.aspx',
  [BudgetTypeEnum.RecreationBudget]:
    'https://erniegh.sharepoint.com/sites/people/benefit/ESK/Pages/Recreational-vouchers.aspx',
  [BudgetTypeEnum.CommunityBudget]: null,
  [BudgetTypeEnum.TeamBudget]: null,
};

@Component({
  selector: 'app-budget',
  templateUrl: './budget.component.html',
  styleUrls: ['./budget.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [ConfirmationService],
})
export class BudgetComponent {
  #requestService = inject(RequestService);
  #budgetService = inject(BudgetService);
  #confirmationService = inject(ConfirmationService);
  #dataChangeNotificationService = inject(DataChangeNotificationService);

  budget = input.required<BudgetModel>();

  budgetTypes = toSignal(this.#budgetService.getBudgetsTypes(), { initialValue: [] });
  budgetTypeName = computed(() => this.budgetTypes().find((type) => type.id == this.budget().type)?.name);
  budgetAmountPercentage = computed(() => (100 * this.budget().amountLeft) / this.budget().amount);
  budgetAmountProgressColor = computed(() => {
    if (!this.budget().isEditable) {
      return '#B7BCC8';
    }
    if (this.budgetAmountPercentage() < 25) {
      return '#D03E35';
    }
    if (this.budgetAmountPercentage() < 60) {
      return '#D0A335';
    }
    return '#55AB55';
  });
  budgetIcon = computed(() => {
    if (!this.budgetTypeName()) {
      return null;
    }

    return budgetTypeToIcon[this.budget().type] ?? null;
  });
  budgetInfoLink = computed(() => {
    return budgetTypeToInfoLink[this.budget().type] ?? null;
  });

  getRequestStateChipColor(state: RequestApprovalState): string | undefined {
    return requestStateChipColor[state];
  }

  requestStateType = RequestApprovalState;

  openDeleteConfirmationModal(request: Request) {
    this.#confirmationService.confirm({
      message: `Are you sure you want to delete the request "<strong><em>${request.title}<em/></strong>"?`,
      header: 'Delete Confirmation',
      icon: 'pi pi-info-circle',
      accept: () => {
        this.#requestService.deleteRequest(request.id).subscribe(() => this.#dataChangeNotificationService.notify());
      },
    });
  }
}
