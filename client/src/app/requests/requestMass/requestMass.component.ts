import { Component, OnInit } from '@angular/core';
import { RequestService } from '../../services/request.service';
import { Request } from '../../model/request/request';
import { Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { UserService } from '../../services/user.service';
import { User } from '../../model/user';
import { UserState } from '../../model/userState';
import { RequestMass } from '../../model/requestMass';
import { BudgetLeft } from '../../model/budgetLeft';
import { AlertService } from '../../services/alert.service';
import { Alert, AlertType } from '../../model/alert.model';


@Component({
    selector: 'app-request-mass',
    templateUrl: './requestMass.component.html',
    styleUrls: ['./requestMass.component.css']
})
export class RequestMassComponent implements OnInit {
    selectedDate: Date;
    requestForm: FormGroup;
    users: User[];
    filteredUsers: User[];
    sufficientBudgetLeftUsers: User[];
    userState: UserState;
    userStateType = UserState;
    private _searchTerm: string;

    addedUsers: User[];

    constructor(private requestService: RequestService,
        private userService: UserService,
        private location: Location,
        private route: ActivatedRoute,
        private fb: FormBuilder,
        private alertService: AlertService,
        private busyIndicatorService: BusyIndicatorService) {
        this.createForm();
    }

    ngOnInit() {
        this.selectedDate = new Date();
        this.userState = <UserState>this.route.snapshot.data['filter'];
        this.addedUsers = [];
        this.sufficientBudgetLeftUsers = [];
        this.getUsers(UserState.Active);
    }

    createForm() {
        this.requestForm = this.fb.group({
            title: ['', Validators.required],
            amount: ['', Validators.required],
        });
    }

    validate(controlName: string): boolean {
        return this.requestForm.controls[controlName].invalid &&
            (this.requestForm.controls[controlName].dirty || this.requestForm.controls[controlName].touched);
    }

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

    usersWithBudgetLeft(): void {
        let amount = 0;
        if (this.requestForm.controls['amount'].value) {
            amount = this.requestForm.controls['amount'].value;
        }

        let request = new BudgetLeft();
        request.amount = amount;
        request.year = this.selectedDate.getFullYear();
        this.requestService.getUsersWithBudgetLeft(request).subscribe(u => this.sufficientBudgetLeftUsers = u);
    }

    getInvalidUsers(): User[] {
        let invalidUsers = this.addedUsers.slice();
        this.sufficientBudgetLeftUsers.forEach(user => {
            invalidUsers = invalidUsers.filter(f => f.id != user.id);
        });
        return invalidUsers;
    }

    invalidUsersExist(): boolean {
        return this.getInvalidUsers().length > 0;
    }

    removeInvalidUsers(): void {
        let invalidUsers = this.getInvalidUsers();
        invalidUsers.forEach(user => {
            this.removeUser(user);
        });
    }

    getUsers(filter: UserState): void {
        this.userService.getAllUsers().subscribe(users => { this.users = users.filter(u => u.state == filter), this.filteredUsers = this.users });
    }

    addUser(user: User, ammount: number): void {
        this.addedUsers.push(user);
    }

    hasBudgetLeft(user: User): boolean {
        return this.sufficientBudgetLeftUsers.some(u => u.id == user.id);;
    }

    removeUser(user: User): void {
        this.addedUsers = this.addedUsers.filter(u => u !== user)
    }

    isUserListValid(): boolean {
        return !this.invalidUsersExist() && this.addedUsers.length > 0;
    }

    isAdded(user: User): boolean {
        return this.addedUsers.some(u => u.id == user.id);
    }

    goBack(): void {
        this.location.back();
    }

    save(title: string, amount: number): void {
        var date = this.selectedDate;
        var users = this.addedUsers;
        this.busyIndicatorService.start();

        let requestData = {
            title: title,
            amount: Number(amount),
            date: date,
            users: users
        } as RequestMass;

        this.requestService.addMassRequest(requestData)
            .subscribe(() => {
                this.alertService.alert(new Alert({ message: "Multiple requests created", type: AlertType.Success, keepAfterRouteChange: true }));
                this.busyIndicatorService.end();
                this.goBack();
            },
                err => {
                    this.alertService.error("Error while creating request: " + JSON.stringify(err.error));
                    this.busyIndicatorService.end();
                }).add(() => this.busyIndicatorService.end());
    }
}