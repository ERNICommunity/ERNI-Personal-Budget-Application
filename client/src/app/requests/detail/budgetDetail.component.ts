import { Component, OnInit } from '@angular/core';
import { User } from '../../model/user';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-users',
  templateUrl: 'budgetDetail.component.html',
  styleUrls: ['budgetDetail.component.css']
})
export class UserDetailComponent implements OnInit {
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
