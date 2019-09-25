import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { User } from '../../model/user';
import { UserService } from '../../services/user.service';

@Component({
    selector: 'app-create-user',
    templateUrl: './create-user.component.html',
    styleUrls: ['./create-user.component.css']
})
export class CreateUserComponent implements OnInit {
    superiors: User[];
    createForm: FormGroup;
    loading: boolean = false;
    submitted: boolean = false;

    constructor(
        private formBuilder: FormBuilder,
        private userService: UserService) {
    }

    ngOnInit() {
        this.createForm = this.formBuilder.group({
            firstName: ['', Validators.required],
            lastName: ['', Validators.required],
            email: ['', [Validators.required, Validators.email]],
            isAdmin: [],
            isSuperior: [],
            superior: []
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

        this.loading = true;

        let userData = {
            firstName: this.controls.firstName.value,
            lastName: this.controls.lastName.value,
            email: this.controls.email.value,
            isAdmin: this.controls.isAdmin.value,
            isSuperior: this.controls.isSuperior.value,
            superior: this.controls.superior.value
        };

        console.log(userData);
        this.loading = false;
    }
}
