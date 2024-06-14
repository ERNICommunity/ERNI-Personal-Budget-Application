import { Component, OnInit } from '@angular/core';
import { User } from '../../model/user';
import { UserService } from '../../services/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { AlertService } from '../../services/alert.service';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-users',
  templateUrl: './userDetail.component.html',
  styleUrls: ['./userDetail.component.css']
})

export class UserDetailComponent implements OnInit {
  id: number;
  user: User;
  users: User[];
  form: UntypedFormGroup;
  submitted = false;



  constructor(private userService: UserService,
    private location: Location,
    private router: Router,
    private route: ActivatedRoute,
    private formBuilder: UntypedFormBuilder,
    private alertService: AlertService,
    private busyIndicatorService: BusyIndicatorService) {
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');

    //    this.userService.getUser(Number(id)).subscribe(user => this.user = user);

      this.form = this.formBuilder.group({
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        superior: [''],
        state: ['', [Validators.required]]
    });

    this.userService.getUser(Number(id)).subscribe(user => {
      this.id = user.id;
      this.form = this.formBuilder.group({
        firstName: [user.firstName, Validators.required],
        lastName: [user.lastName, Validators.required],
        email: [user.email, [Validators.required, Validators.email]],
        superior: [user.superior.id],
        state: [user.state, [Validators.required]]
      });
    });



    this.userService.getAllUsers().subscribe(users => this.users = users.sort((first, second) => first.lastName.localeCompare(second.lastName)));
  }

  trimControlValue(control) {
    control.setValue(control.value.trim());
  }

  compareUsers(user1: User, user2: User) {
    return user1 && user2 ? user1.id === user2.id : user1 === user2;
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

    this.busyIndicatorService.start();

    const userData = {
      id: this.id,
      firstName: this.form.controls.firstName.value,
      lastName: this.form.controls.lastName.value,
      email: this.form.controls.email.value,
      superior: Number(this.form.controls.superior.value),
      state: Number(this.form.controls.state.value)
    };

    this.userService.updateUser(userData).subscribe(
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
      }
    ).add(() => {
      this.busyIndicatorService.end();
    });
  }

}
