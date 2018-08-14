import {Component, OnInit} from '@angular/core';
import {UserService} from '../../services/user.service';
import {User} from '../../model/user';
import { ActivatedRoute } from '@angular/router';
import { UserState } from '../../model/userState';

@Component({
    selector: 'app-users',
    templateUrl: './userList.component.html',
    styleUrls: ['./userList.component.css']
})
export class UserListComponent implements OnInit {
    users: User[];

    constructor(private valueService: UserService, private route: ActivatedRoute) {
    }

    ngOnInit() {
        var filter = <UserState>this.route.snapshot.data['filter'];

        this.getUsers(filter);
    }

    getUsers(filter: UserState): void {
        this.valueService.getRequests()
            .subscribe(users => this.users = users.filter(u => u.state == filter));
    }

}
