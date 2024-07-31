import { Component, OnInit } from '@angular/core';
import { UntypedFormGroup, UntypedFormBuilder, Validators, AbstractControl } from '@angular/forms';
import { User } from '../../model/user';
import { UserService } from '../../services/user.service';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { Router } from '@angular/router';
import { AlertService } from '../../services/alert.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.css'],
})
export class CreateUserComponent implements OnInit {
  superiors: User[];
  createForm: UntypedFormGroup;
  submitted = false;
  errorMessage: string;

  constructor(
    private router: Router,
    private formBuilder: UntypedFormBuilder,
    private userService: UserService,
    private alertService: AlertService,
    public busyIndicatorService: BusyIndicatorService,
  ) {}

  ngOnInit() {
    this.createForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      superior: [''],
      state: ['', [Validators.required]],
    });

    this.userService.getAllUsers().subscribe((users) => {
      this.superiors = users.sort((first, second) => first.lastName.localeCompare(second.lastName));
    });
  }

  get controls() {
    return this.createForm.controls;
  }

  trimControlValue(control: AbstractControl) {
    control.setValue(control.value.trim());
  }

  onSubmit() {
    this.alertService.clear();
    this.submitted = true;

    if (this.createForm.invalid) {
      return;
    }

    this.busyIndicatorService.start();

    const userData = {
      firstName: this.controls.firstName.value,
      lastName: this.controls.lastName.value,
      email: this.controls.email.value,
      superior: Number(this.controls.superior.value),
      state: Number(this.controls.state.value),
    };

    this.userService
      .createUser(userData)
      .subscribe(
        () => {
          this.alertService.success('User successfully was created.');
          this.router.navigate(['/users']);
        },
        (err: HttpErrorResponse) => {
          let error = 'User was not created.';
          if (err.status === 409) {
            error = 'User is already exists.';
          }

          this.alertService.error(error);
        },
      )
      .add(() => {
        this.busyIndicatorService.end();
      });
  }
}
