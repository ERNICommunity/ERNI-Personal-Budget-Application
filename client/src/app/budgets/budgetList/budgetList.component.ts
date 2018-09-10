import { Component, OnInit } from '@angular/core';
import { User } from '../../model/user';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-users',
  templateUrl: 'budgetList.component.html',
  styleUrls: ['budgetList.component.css']
})
export class UserListComponent implements OnInit {
  users: User[];

  constructor(private valueService: UserService) { }

  ngOnInit() {
    this.getHeroes();
  }

  getHeroes(): void {
    // this.valueService.getUsers()
    //   .subscribe(users => this.users = users);
  }

}
