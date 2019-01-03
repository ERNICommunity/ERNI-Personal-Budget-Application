import { Component } from '@angular/core';
import { AdalService } from './services/adal.service';
import { UserService } from './services/user.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  isAdmin: boolean;
  isSuperior: boolean;
  initialized: boolean;

  constructor(public adalService: AdalService, private userService: UserService) {
    this.initialized = false;
  }

  ngDoCheck() {
    if (!this.initialized && this.adalService.userInfo)
    {
      this.getIsAdminOrSuperior();
      this.initialized = true;
    }  
  }

  getIsAdminOrSuperior() : void {
     this.userService.getCurrentUser().subscribe(u => 
      {
        this.isAdmin = u.isAdmin;
        if (!this.isAdmin)
        {
          this.userService.getSubordinateUsers().subscribe(users => this.isSuperior = users != null && users.length > 0);
        }
      });
   }
}
