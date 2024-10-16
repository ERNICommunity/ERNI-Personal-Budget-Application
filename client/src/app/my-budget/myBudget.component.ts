import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { BudgetService } from '../services/budget.service';
import { ActivatedRoute } from '@angular/router';
import { combineLatest } from 'rxjs';
import { DataChangeNotificationService } from '../services/dataChangeNotification.service';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { MenuHelper } from '../shared/menu-helper';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-my-budget',
  templateUrl: './myBudget.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyBudgetComponent {
  #budgetService = inject(BudgetService);
  #route = inject(ActivatedRoute);
  #dataChangeNotificationService = inject(DataChangeNotificationService);

  loadingData = signal(false);
  budgets = toSignal(
    combineLatest([this.#route.params, this.#dataChangeNotificationService.notifications$]).pipe(
      map(([params]) => Number(params['year'])),
      filter((year) => !isNaN(year)),
      tap((_) => this.loadingData.set(true)),
      switchMap((year) => this.#budgetService.getCurrentUserBudgets(year)),
      tap((_) => this.loadingData.set(false)),
    ),
    { initialValue: [] },
  );

  years = MenuHelper.getYearMenu((year) => ['/my-budget', year]);
}
