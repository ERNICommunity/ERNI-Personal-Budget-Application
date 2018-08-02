import { Component, OnInit } from '@angular/core';
import { User } from '../../model/user';
import { UserService } from '../../services/user.service';
import { RequestService } from '../../services/request.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-users',
  templateUrl: './userDetail.component.html',
  styleUrls: ['./userDetail.component.css']
})
export class UserDetailComponent implements OnInit {
  user: User;
  users: User[];

  constructor(private userService: UserService, private requestService: RequestService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.getHeroes();
  }

  getHeroes(): void {

    const id = this.route.snapshot.paramMap.get('id');

    this.userService.getUser(Number(id))
      .subscribe(user => this.user = user);

      this.userService.getRequests()
      .subscribe(users => this.users = users);
  }

  save() : void {
    this.userService.updateUser(this.user);
      // .subscribe(() => this.goBack())
  }
}
