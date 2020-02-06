import { Component, OnInit, Input } from '@angular/core';
import { BudgetService } from '../../services/budget.service';
import { TeamBudget } from '../../model/request/team-budget';
import { RequestFilter } from '../../requests/requestFilter';
import { ActivatedRoute, Params } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { RequestService } from '../../services/request.service';

@Component({
    selector: 'app-team-budget',
    templateUrl: './team-budget.component.html',
    styleUrls: ['./team-budget.component.css']
})
export class TeamBudgetComponent implements OnInit {
    budget: TeamBudget;
    requestStateType = RequestFilter;

    year: number;

    constructor(
        private route: ActivatedRoute,
        private budgetService: BudgetService,
        private requestService: RequestService,
        private modalService: NgbModal) { }

    ngOnInit() {
        this.route.params.subscribe((params: Params) => {
            this.year = params['year'];
            this.initializeTeamBudgets(this.year);
        });
    }

    openDeleteConfirmationModal(content) {
        this.modalService.open(content, { centered: true, backdrop: 'static' });
    }

    deleteRequest(id: number): void {
        this.requestService.deleteTeamRequest(id).subscribe(() => {
            this.initializeTeamBudgets(this.year);
        });
    }

    private initializeTeamBudgets(year: number): void {
        this.budgetService.getTeamBudgets(year).subscribe(budget => {
            this.budget = budget;
        });
    }
}
