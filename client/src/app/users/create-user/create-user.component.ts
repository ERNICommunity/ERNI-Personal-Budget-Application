import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { User } from '../../model/user';
import { UserService } from '../../services/user.service';
import { ConfigService } from '../../services/config.service';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { Router } from '@angular/router';
import { AlertService } from '../../services/alert.service';
import { HttpErrorResponse } from '@angular/common/http';

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
        private alertService: AlertService,
        public busyIndicatorService: BusyIndicatorService) {
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
            amount: [''],
            year: [this.currentYear],
            isAdmin: [false],
            isSuperior: [false],
            superior: [''],
            state: ['', [Validators.required]]
        });

        this.createForm.get('state').valueChanges.subscribe(state => {
            this.controls.amount.setValidators(null);
            this.controls.year.setValidators(null);

            if (state == 1) {
                this.controls.amount.setValidators([Validators.required]);
                this.controls.year.setValidators([Validators.required]);
            }

            this.controls.amount.updateValueAndValidity();
            this.controls.year.updateValueAndValidity();
        });

        this.userService.getSubordinateUsers().subscribe(
            users => {
                this.superiors = users.filter(u => u.isSuperior).sort((first, second) => first.lastName.localeCompare(second.lastName));
            });
    }

    get controls() {
        return this.createForm.controls;
    }

    trimControlValue(control) {
        control.setValue(control.value.trim());
    }

    onSubmit() {
        this.alertService.clear();
        this.submitted = true;

        if (this.createForm.invalid) {
            return;
        }

        this.busyIndicatorService.start();

        let userData = {
            firstName: this.controls.firstName.value,
            lastName: this.controls.lastName.value,
            email: this.controls.email.value,
            amount: this.controls.amount.value ? this.controls.amount.value : 0,
            year: this.controls.year.value ? this.controls.year.value : 0,
            isAdmin: this.controls.isAdmin.value,
            isSuperior: this.controls.isSuperior.value,
            isViewer: false,
            superior: Number(this.controls.superior.value),
            state: Number(this.controls.state.value)
        };

        this.userService.createUser(userData).subscribe(
            () => {
                this.alertService.success("User successfully was created.");
                this.router.navigate(['/users']);
            },
            (err: HttpErrorResponse) => {
                let error = "User was not created.";
                if (err.status === 409)
                    error = "User is already exists.";

                this.alertService.error(error);
            }
        ).add(() => {
            this.busyIndicatorService.end()
        });
    }
}
