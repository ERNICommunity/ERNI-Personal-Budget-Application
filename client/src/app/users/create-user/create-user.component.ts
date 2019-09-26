import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { User } from '../../model/user';
import { UserService } from '../../services/user.service';
import { ConfigService } from '../../services/config.service';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-create-user',
    templateUrl: './create-user.component.html',
    styleUrls: ['./create-user.component.css']
})
export class CreateUserComponent implements OnInit {
    superiors: User[];
    years: number[];
    currentYear: number;
    createForm: FormGroup;
    submitted: boolean = false;
    errorMessage: string;

    constructor(
        private router: Router,
        private formBuilder: FormBuilder,
        private userService: UserService,
        private configService: ConfigService,
        private busyIndicatorService: BusyIndicatorService) {
    }

    ngOnInit() {
        this.years = [];
        this.currentYear = (new Date()).getFullYear();

        for (var year = this.currentYear; year >= this.configService.getOldestYear; year--) {
            this.years.push(year);
        }

        this.createForm = this.formBuilder.group({
            firstName: ['', Validators.required],
            lastName: ['', Validators.required],
            email: ['', [Validators.required, Validators.email]],
            amount: ['', Validators.required],
            year: [this.currentYear, [Validators.required]],
            isAdmin: [false],
            isSuperior: [false],
            superior: [],
            state: ['', [Validators.required]]
        });

        this.userService.getSubordinateUsers().subscribe(
            users => {
                this.superiors = users.filter(u => u.isSuperior).sort((first, second) => first.lastName.localeCompare(second.lastName));
            });
    }

    get controls() {
        return this.createForm.controls;
    }

    onSubmit() {
        this.submitted = true;

        if (this.createForm.invalid)
            return;

        this.busyIndicatorService.start();

        let userData = {
            firstName: this.controls.firstName.value,
            lastName: this.controls.lastName.value,
            email: this.controls.email.value,
            amount: this.controls.amount.value,
            year: this.controls.year.value,
            isAdmin: this.controls.isAdmin.value,
            isSuperior: this.controls.isSuperior.value,
            isViewer: false,
            superior: this.controls.superior.value,
            state: this.controls.state.value
        };

        this.userService.createUser(userData).subscribe(
            () => {
                this.router.navigate(['/users']);
            },
            () => {
                this.errorMessage = "User was not created."
            }
        ).add(() => {
            this.busyIndicatorService.end()
        });
    }
}
