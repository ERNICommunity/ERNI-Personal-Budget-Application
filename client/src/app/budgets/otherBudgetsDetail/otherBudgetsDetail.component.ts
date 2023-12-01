import { Component, OnInit } from "@angular/core";
import { Location, NgIf, NgFor } from "@angular/common";
import { Budget } from "../../model/budget";
import { BudgetService } from "../../services/budget.service";
import { ActivatedRoute } from "@angular/router";
import { User } from "../../model/user";
import { FormsModule } from "@angular/forms";

@Component({
  selector: "app-other-budgets-detail",
  templateUrl: "otherBudgetsDetail.component.html",
  styleUrls: ["otherBudgetsDetail.component.css"],
  standalone: true,
  imports: [NgIf, FormsModule, NgFor],
})
export class OtherBudgetsDetailComponent implements OnInit {
  budget!: Budget;
  availableUsers: User[] = [];
  selectedUserId?: number;

  constructor(
    private budgetService: BudgetService,
    private location: Location,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.getUserBudget();
  }

  getUserBudget(): void {
    const id = this.route.snapshot.paramMap.get("id");
    this.budgetService.getBudget(parseInt(id!)).subscribe((budget) => {
      this.budget = budget;
      this.selectedUserId = budget.user.id;
      this.getAvailableUsers();
    });
  }

  getAvailableUsers(): void {
    this.budgetService
      .getUsersAvailableForBudgetType(this.budget.type)
      .subscribe((users) => (this.availableUsers = users));
  }

  goBack(): void {
    this.location.back();
  }

  save(): void {
    this.budgetService
      .updateBudget(this.budget.id, this.budget.amount)
      .subscribe(() => {
        if (this.selectedUserId != this.budget.user.id) {
          this.budgetService
            .transferBudget(this.budget.id, this.selectedUserId!)
            .subscribe();
        }
        this.goBack();
      });
  }
}
