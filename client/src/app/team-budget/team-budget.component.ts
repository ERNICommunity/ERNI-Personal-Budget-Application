import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api/menuitem';
import { MenuHelper } from '../shared/menu-helper';
import { RouterOutlet } from '@angular/router';
import { TeamRequestComponent } from './team-request/team-request.component';
import { TeamBudgetStateComponent } from './team-budget-state/team-budget-state.component';
import { TabMenuModule } from 'primeng/tabmenu';

@Component({
    selector: 'app-team-budget',
    templateUrl: './team-budget.component.html',
    styleUrls: ['./team-budget.component.css'],
    standalone: true,
    imports: [TabMenuModule, TeamBudgetStateComponent, TeamRequestComponent, RouterOutlet]
})
export class TeamBudgetComponent {
    years: MenuItem[];

    constructor() {
        this.years = MenuHelper.getYearMenu((year) => ['/team-budget', year]);
    }
}
