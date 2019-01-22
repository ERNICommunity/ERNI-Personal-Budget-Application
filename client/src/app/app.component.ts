import { Component } from '@angular/core';
import { AdalService } from './services/adal.service';
import { UserService } from './services/user.service';
import { Router, NavigationStart, NavigationCancel, NavigationError, NavigationEnd } from '@angular/router';
import { BusyIndicatorService } from './services/busy-indicator.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  isAdmin: boolean;
  isSuperior: boolean;
  initialized: boolean;

  constructor(public adalService: AdalService, private userService: UserService, private router: Router, public busyIndicatorService: BusyIndicatorService) {
    this.initialized = false;

    this.router.events.subscribe(event => {
      switch (true) {
        case event instanceof NavigationStart: {
          this.busyIndicatorService.start();
          break;
        }
        case event instanceof NavigationEnd:
        case event instanceof NavigationCancel:
        case event instanceof NavigationError: {
          this.busyIndicatorService.end();
          break;
        }
        default: {
          break;
        }
      }
    });
  }

  ngDoCheck() {
    if (!this.initialized && this.adalService.userInfo) {
      this.getIsAdminOrSuperior();
      this.initialized = true;
    }
  }

  getIsAdminOrSuperior(): void {
    this.userService.getCurrentUser().subscribe(u => {
      this.isAdmin = u.isAdmin;
      if (!this.isAdmin) {
        this.userService.getSubordinateUsers().subscribe(users => this.isSuperior = users != null && users.length > 0);
      }
    });
  }

  logout() {
    this.adalService.logout();
  }
}
