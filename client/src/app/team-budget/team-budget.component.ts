import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api/menuitem';
import { MenuHelper } from '../shared/menu-helper';

@Component({
    selector: 'app-team-budget',
    templateUrl: './team-budget.component.html'
})
export class TeamBudgetComponent {
    years: MenuItem[];

    constructor() {
        this.years = MenuHelper.getYearMenu((year) => ['/team-budget', year]);
    }
}
