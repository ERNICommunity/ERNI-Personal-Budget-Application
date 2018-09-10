import {Component, OnInit} from '@angular/core';
import {User} from '../../model/user';
import {UserService} from '../../services/user.service';
import {BudgetService} from '../../services/budget.service';
import {RequestService} from "../../services/request.service";
import {Request} from "../../model/request";

@Component({
    selector: 'app-users',
    templateUrl: 'budgetDetail.component.html',
    styleUrls: ['budgetDetail.component.css']
})
export class UserDetailComponent implements OnInit {
    users: User[];
    requests: Request[];

    constructor(private userService: UserService,
                private budgetService: BudgetService,
                private requestService: RequestService) {
    }

    ngOnInit() {
        this.getRequests();
    }

    getRequests(): void {
        // this.requestService.getRequests(2018)
        //     .subscribe(requests => this.requests = requests);
    }

}
