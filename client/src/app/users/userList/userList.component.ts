import { Component, OnInit} from '@angular/core';
import { UserService } from '../../services/user.service';
import { User } from '../../model/user';
import { ActivatedRoute } from '@angular/router';
import { UserState } from '../../model/userState';

@Component({
    selector: 'app-users',
    templateUrl: './userList.component.html',
    styleUrls: ['./userList.component.css']
})
export class UserListComponent implements OnInit {
    users: User[];
    userState : UserState;
    userStateType = UserState;

    constructor(private userService: UserService, private route: ActivatedRoute) {
    }

    ngOnInit() {
        this.userState = <UserState>this.route.snapshot.data['filter'];
        this.getUsers(this.userState);
    }

    getUsers(filter: UserState): void {
        this.userService.getSubordinateUsers().subscribe(users => this.users = users.filter(u => u.state == filter));
    }

    activateEmployee(user: User): void {
        this.users = this.users.filter(u => u.id !== user.id);
        user.state = UserState.Active;
        this.userService.updateUser(user).subscribe(); 
    }

    deactivateEmployee(user: User): void {
        this.users = this.users.filter(u => u.id !== user.id);
        user.state = UserState.Inactive;
        this.userService.updateUser(user).subscribe();
    }
}
