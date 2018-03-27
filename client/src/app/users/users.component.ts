import { Component, OnInit } from '@angular/core';
import { User } from '../model/user';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
  users: User[];

  constructor(private valueService: UserService) { }

  ngOnInit() {
    this.getHeroes();
  }

  getHeroes(): void {
    this.valueService.getRequests()
      .subscribe(users => this.users = users);
  }

}
