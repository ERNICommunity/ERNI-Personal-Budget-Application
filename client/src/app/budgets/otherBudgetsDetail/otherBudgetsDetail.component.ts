import { Component, inject } from "@angular/core";
import { Location } from "@angular/common";
import { Budget } from "../../model/budget";
import { BudgetService } from "../../services/budget.service";
import { ActivatedRoute } from "@angular/router";
import { User } from "../../model/user";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { map, shareReplay, switchMap, tap } from "rxjs";
import { SharedModule } from "../../shared/shared.module";

@Component({
  selector: "app-other-budgets-detail",
  templateUrl: "otherBudgetsDetail.component.html",
  styleUrls: ["otherBudgetsDetail.component.css"],
  standalone: true,
  imports: [SharedModule],
})
export class OtherBudgetsDetailComponent {
  availableUsers: User[] = [];

  #budgetService = inject(BudgetService);
  #location = inject(Location);
  #route = inject(ActivatedRoute);

  budget: Budget | null = null;
  form = new FormGroup({
    amount: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required],
    }),
    user: new FormControl<number | null>(null, {
      validators: [Validators.required],
    }),
  });

  constructor() {
    const budget$ = this.#route.paramMap.pipe(
      map((params) => params.get("id") ?? ""),
      switchMap((id) => this.#budgetService.getBudget(parseInt(id))),
      tap((budget) => (this.budget = budget)),
      shareReplay(1)
    );

    budget$
      .pipe(
        switchMap((budget) =>
          this.#budgetService.getUsersAvailableForBudgetType(budget.type)
        )
      )
      .subscribe((users) => (this.availableUsers = users));

    budget$.subscribe((budget) => {
      this.form.setValue({
        amount: budget.amount,
        user: budget.user.id,
      });
    });
  }

  goBack() {
    this.#location.back();
  }

  save() {
    const budget = this.budget;

    const value = this.form.getRawValue();
    const user = value.user;

    if (!budget || !user) {
      return;
    }

    this.#budgetService.updateBudget(budget.id, value.amount).subscribe(() => {
      if (value.user != budget.user.id) {
        this.#budgetService.transferBudget(budget.id, user).subscribe();
      }
      this.goBack();
    });
  }
}
