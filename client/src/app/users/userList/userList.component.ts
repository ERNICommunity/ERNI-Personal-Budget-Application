import {Component, OnInit} from '@angular/core';
import {UserService} from '../../services/user.service';
import {User} from '../../model/user';

@Component({
    selector: 'app-users',
    templateUrl: './userList.component.html',
    styleUrls: ['./userList.component.css']
})
export class UserListComponent implements OnInit {
    users: User[];

    constructor(private valueService: UserService) {
    }

    ngOnInit() {
        this.getUsers();
    }

    getUsers(): void {
        this.valueService.getRequests()
            .subscribe(users => this.users = users);
    }

}
