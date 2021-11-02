import { Component, OnInit } from '@angular/core';
import { User } from '../../model/user';
import { UserService } from '../../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-users',
  templateUrl: './userDetail.component.html',
  styleUrls: ['./userDetail.component.css']
})

export class UserDetailComponent implements OnInit {
  user: User;
  users: User[];
  form: FormGroup;


  constructor(private userService: UserService, private location: Location, private route: ActivatedRoute, private formBuilder: FormBuilder) {

    this.form = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      superior: [''],
      state: ['', [Validators.required]]
  });
    // this.form = this.formBuilder.group({
    //   firstName: ['', Validators.required],
    //   lastName: ['', Validators.required],
    //   email: ['', [Validators.required, Validators.email]],
    //   superior: [''],
    //   state: ['', [Validators.required]]
    // });
    

  }

  ngOnInit() {

    const id = this.route.snapshot.paramMap.get('id');

this.form = this.formBuilder.group({
  firstName: ['', Validators.required],
  lastName: ['', Validators.required],
  email: ['', [Validators.required, Validators.email]],
  superior: [''],
  state: ['', [Validators.required]]
});

    // this.userService.getUser(Number(id)).subscribe(user => {
    //   c.form = this.formBuilder.group({
    //     firstName: [user.firstName, Validators.required],
    //     lastName: [user.lastName, Validators.required],
    //     email: [user.email, [Validators.required, Validators.email]],
    //     superior: [user.superior.id],
    //     state: [user.state, [Validators.required]]
    //   });
    // });




    this.userService.getAllUsers().subscribe(users => this.users = users.sort((first, second) => first.lastName.localeCompare(second.lastName)));
  }

  compareUsers(user1: User, user2: User) {
    return user1 && user2 ? user1.id === user2.id : user1 === user2;
  }

  goBack(): void {
    this.location.back();
  }

  save(): void {
    this.userService.updateUser(this.user).subscribe();
    this.location.back();
  }
}
