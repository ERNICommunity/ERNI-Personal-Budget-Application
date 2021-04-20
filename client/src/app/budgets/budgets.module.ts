import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { AdminRoleGuard } from "../services/guards/admin-role.guard";
import { OtherBudgetsDetailComponent } from "./otherBudgetsDetail/otherBudgetsDetail.component";
import { OtherBudgetsComponent } from "./otherBudgets/otherBudgets.component";
import { FormsModule } from "@angular/forms";

const currentYear = "2021"; // = (new Date()).getFullYear();

@NgModule({
  declarations: [OtherBudgetsComponent, OtherBudgetsDetailComponent],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild([
      {
        path: "other-budgets", canActivate: [AdminRoleGuard],
        children: [
          { path: "", redirectTo: currentYear + "/1", pathMatch: "full" },
          { path: "edit/:id", component: OtherBudgetsDetailComponent, canActivate: [AdminRoleGuard], },
          { path: ":year/:budgetType", component: OtherBudgetsComponent, canActivate: [AdminRoleGuard], },
        ],
      },
    ]),
  ],
})
export class BudgetsModule {}
