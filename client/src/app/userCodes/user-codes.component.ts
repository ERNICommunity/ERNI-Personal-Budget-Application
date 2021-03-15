import { Component, OnInit } from '@angular/core';
import { UserCodeService } from '../services/user-code.service';
import { UserCode } from '../model/userCode';

@Component({
    templateUrl: './user-codes.component.html'
})
export class UserCodesComponent implements OnInit {
    public users: UserCode[];
    public filteredUsers: UserCode[];

    private _searchTerm: string;

    get searchTerm(): string {
        return this._searchTerm;
    }

    set searchTerm(value: string) {
        this._searchTerm = value;
        this.filteredUsers = this.filterUsers(value);
    }

    constructor(private userCodeService: UserCodeService) { }

    ngOnInit() {

        this.userCodeService.getUserCodes().subscribe(userCodes => {
            this.users = userCodes;
            this.filteredUsers = userCodes;
        });
    }

    
    filterUsers(searchString: string) {
        searchString = searchString.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "");

        return this.users.filter(user => {

            console.log(user);

            return user.firstName && user.firstName.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString) !== -1 ||
            user.lastName && user.lastName.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString) !== -1 ||
            user.displayName && user.displayName.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString) !== -1 ||
            user.code && user.code.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, "").indexOf(searchString) !== -1;
        });
    }
}