import { Component } from '@angular/core';
import { AdalService } from './services/adal.service';
import { UserService } from './services/user.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Zeroes app';
  isAdmin: boolean;
  isSuperior: boolean;
  initialized: boolean;

  constructor(private adalService: AdalService, private userService: UserService) {
    this.initialized = false;
  }

  ngDoCheck() {
    // console.log("ngDoCheck");
    if (!this.initialized && this.adalService.userInfo)
    {
      // console.log("initializing");
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
          this.userService.getUsers().subscribe(users => this.isSuperior = users != null && users.length > 0);
        }
      });
   }
}
