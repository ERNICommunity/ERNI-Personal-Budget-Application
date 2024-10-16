import { ChangeDetectionStrategy, Component, effect, inject, signal } from '@angular/core';
import { Location } from '@angular/common';
import { BudgetService } from '../../services/budget.service';
import { ActivatedRoute } from '@angular/router';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { of, switchMap } from 'rxjs';

@Component({
  selector: 'app-other-budgets-detail',
  templateUrl: 'otherBudgetsDetail.component.html',
  styleUrls: ['otherBudgetsDetail.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OtherBudgetsDetailComponent {
  selectedUserId = signal<number>(0);

  #budgetService = inject(BudgetService);
  #location = inject(Location);

  budget = toSignal(
    inject(ActivatedRoute).paramMap.pipe(
      switchMap((paramMap) => {
        const id = paramMap.get('id');

        if (!id) {
          return of(undefined);
        }

        return this.#budgetService.getBudget(parseInt(id));
      }),
    ),
  );

  constructor() {
    effect(
      () => {
        const budget = this.budget();
        if (budget) {
          this.selectedUserId.set(budget.user.id);
        }
      },
      { allowSignalWrites: true },
    );
  }

  public readonly availableUsers = toSignal(
    toObservable(this.budget).pipe(
      switchMap((budget) => (budget ? this.#budgetService.getUsersAvailableForBudgetType(budget.type) : [])),
    ),
  );

  goBack(): void {
    this.#location.back();
  }

  save(): void {
    const budget = this.budget();
    const selectedUserId = this.selectedUserId();

    if (!budget) {
      return;
    }

    this.#budgetService.updateBudget(budget.id, budget.amount).subscribe(() => {
      if (selectedUserId !== budget.user.id) {
        this.#budgetService.transferBudget(budget.id, selectedUserId).subscribe();
      }
      this.goBack();
    });
  }
}
