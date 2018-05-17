import { Component, OnInit } from '@angular/core';
import { User } from '../../model/user';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-users',
  templateUrl: './userList.component.html',
  styleUrls: ['./userList.component.css']
})
export class UserListComponent implements OnInit {
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
