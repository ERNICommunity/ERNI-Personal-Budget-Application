import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { RequestService } from '../../services/request.service';
import { Category } from '../../model/category';
import { Request } from '../../model/request';
import { Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { UserService } from '../../services/user.service';
import { User } from '../../model/user';
import { UserState } from '../../model/userState';
import { RequestMass } from '../../model/requestMass';
import { BudgetLeft } from '../../model/budgetLeft';


@Component({
    selector: 'app-request-mass',
    templateUrl: './requestMass.component.html',
    styleUrls: ['./requestMass.component.css']
})
export class RequestMassComponent implements OnInit {
    categories: Category[];
    selectedCategory: Category;
    httpResponseError: string;
    selectedDate: Date;
    requestForm: FormGroup;
    requestUrl: string;
    users: User[];
    filteredUsers: User[];
    userState: UserState;
    userStateType = UserState;
    private _searchTerm: string;

    addedUsers: User[];

    constructor(private categoryService: CategoryService,
        private requestService: RequestService,
        private userService: UserService,
        private location: Location,
        private route: ActivatedRoute,
        private fb: FormBuilder,
        private busyIndicatorService: BusyIndicatorService) {
        this.createForm();
    }

    ngOnInit() {
        this.onChanges();
        this.getCategories();
        this.selectedDate = new Date();
        this.userState = <UserState>this.route.snapshot.data['filter'];
        this.addedUsers = [];
        this.requestForm.get('url').disable();
        this.getUsers(UserState.Active);
    }

    createForm() {
        this.requestForm = this.fb.group({
            title: ['', Validators.required],
            amount: ['', Validators.required],
            category: ['', Validators.required],
            url: ['', Validators.required]
        });
    }

    onChanges() {
        this.requestForm.get('category').valueChanges
            .subscribe(selectedCategory => {
                if (selectedCategory.isUrlNeeded) {
                    this.requestForm.get('url').enable();
                }
                else {
                    this.requestForm.get('url').disable();
                    this.requestForm.get('url').reset();
                }
            });
    }

    getCategories(): void {

        this.busyIndicatorService.start();
        this.categoryService.getCategories()
            .subscribe(categories => {
                this.categories = categories.filter(cat => cat.isActive == true),
                    this.selectedCategory = categories.filter(cat => cat.isActive == true)[0];
                this.busyIndicatorService.end();
            });
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

    getUsers(filter: UserState): void {
        this.userService.getSubordinateUsers().subscribe(users => { this.users = users.filter(u => u.state == filter), this.filteredUsers = this.users });
    }

    addUser(user: User, ammount: number): void {
        let request = new BudgetLeft();
        request.amount = ammount;
        request.categoryId = this.selectedCategory.id;
        request.id = user.id;
        request.requestId = null;
        request.year = this.selectedDate.getFullYear();
        let hasBudgetLeft: boolean;
        this.requestService.hasBudgetLeft(request).subscribe(r => {
            if (r) {
                this.addedUsers.push(user);
            }
        });
    }

    removeUser(user: User): void {
        this.addedUsers = this.addedUsers.filter(u => u !== user)
    }

    isAdded(user: User): boolean {
        return this.addedUsers.some(u => u == user);
    }

    goBack(): void {
        this.location.back();
    }

    save(title: string, amount: number): void {
        var category = this.selectedCategory;
        var date = this.selectedDate;
        var url = this.requestUrl;
        var users = this.addedUsers;
        this.busyIndicatorService.start();

        this.requestService.addRequestMass({ title, amount, date, category, url, users } as RequestMass)
            .subscribe(() => {
                this.busyIndicatorService.end();
                this.goBack();
            },
                err => {
                    this.httpResponseError = JSON.stringify(err.error);
                    this.busyIndicatorService.end();
                })
    }
}