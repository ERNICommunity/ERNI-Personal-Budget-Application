import { Component, OnInit } from '@angular/core';
import { User } from '../../model/user';
import { UserService } from '../../services/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { AbstractControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { AlertService } from '../../services/alert.service';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-users',
  templateUrl: './userDetail.component.html',
})
export class UserDetailComponent implements OnInit {
  id: number;
  user: User;
  users: User[];
  form: UntypedFormGroup = this.formBuilder.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    superior: [''],
    state: ['', [Validators.required]],
  });
  submitted = false;

  constructor(
    private userService: UserService,
    private location: Location,
    private router: Router,
    private route: ActivatedRoute,
    private formBuilder: UntypedFormBuilder,
    private alertService: AlertService,
    private busyIndicatorService: BusyIndicatorService,
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');

    this.userService.getUser(Number(id)).subscribe((user) => {
      this.id = user.id;
      this.form = this.formBuilder.group({
        firstName: [user.firstName, Validators.required],
        lastName: [user.lastName, Validators.required],
        email: [user.email, [Validators.required, Validators.email]],
        superior: [user.superior.id],
        state: [user.state, [Validators.required]],
      });
    });

    this.userService.getAllUsers().subscribe((users) => {
      this.users = users.sort((first, second) => first.lastName.localeCompare(second.lastName));
    });
  }

  trimControlValue(control: AbstractControl) {
    control.setValue(control.value.trim());
  }

  goBack(): void {
    this.location.back();
  }

  onSubmit() {
    this.alertService.clear();
    this.submitted = true;

    if (this.form.invalid) {
      return;
    }

    const userData = {
      id: this.id,
      firstName: this.form.controls.firstName.value,
      lastName: this.form.controls.lastName.value,
      email: this.form.controls.email.value,
      superior: Number(this.form.controls.superior.value),
      state: Number(this.form.controls.state.value),
    };

    this.busyIndicatorService.start();

    this.userService
      .updateUser(userData)
      .pipe(finalize(() => this.busyIndicatorService.end()))
      .subscribe(
        () => {
          this.alertService.success('User successfully created');
          this.router.navigate(['/users']);
        },
        (err: HttpErrorResponse) => {
          let error = 'User was not created.';
          if (err.status === 409) {
            error = 'User is already exists.';
          }

          this.alertService.error(error);
        },
      );
  }
}
