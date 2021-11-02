import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { User } from '../../model/user';
import { ActivatedRoute, Router } from '@angular/router';
import { UserState } from '../../model/userState';

@Component({
    selector: 'app-users-list',
    templateUrl: './userList.component.html',
    styleUrls: ['./userList.component.css']
})
export class UserListComponent implements OnInit {
    users: User[];
    filteredUsers: User[];
    userState: UserState;
    userStateType = UserState;

    private _searchTerm: string;

    get searchTerm(): string {
        return this._searchTerm;
    }

    set searchTerm(value: string) {
        this._searchTerm = value;
        this.filteredUsers = this.filterUsers(value);
    }

    filterUsers(searchString: string) {
        searchString = searchString.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "");

        return this.users.filter(user => user.firstName.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString) !== -1 ||
            user.lastName.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString) !== -1);
    }

    constructor(private userService: UserService, private route: ActivatedRoute, private router: Router) {
    }

    ngOnInit() {
        this.userState = <UserState>this.route.snapshot.data['filter'];
        this.getUsers(this.userState);
    }

    getUsers(filter: UserState): void {
        this.userService.getAllUsers().subscribe(users => { 
            this.users = users.filter(u => u.state == filter), 
            this.filteredUsers = this.users 
        });
    }

    activateEmployee(user: User): void {
        user.state = UserState.Active;
        this.userService.updateUser(user).subscribe(() => { this.users = this.users.filter(u => u.id !== user.id), this.filteredUsers = this.users });
    }

    deactivateEmployee(user: User): void {
        user.state = UserState.Inactive;
        this.userService.updateUser(user).subscribe(() => { this.users = this.users.filter(u => u.id !== user.id), this.filteredUsers = this.users });
    }

    create() {
        this.router.navigate(['/users/create']);
    }
}
