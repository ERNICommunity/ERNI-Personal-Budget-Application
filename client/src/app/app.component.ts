import { Component, DoCheck, OnInit } from '@angular/core';
import { AdalService } from './services/adal.service';
import { UserService } from './services/user.service';
import { Router, NavigationStart, NavigationCancel, NavigationError, NavigationEnd } from '@angular/router';
import { BusyIndicatorService } from './services/busy-indicator.service';
import { User } from './model/user';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements DoCheck {
    user: User;
    initialized: boolean;

    constructor(public adalService: AdalService, private userService: UserService, private router: Router, public busyIndicatorService: BusyIndicatorService) {
        this.initialized = false;
        this.user = new User();

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
        var _this = this;
        this.userService.getCurrentUser().subscribe(u => {
            _this.user = u;
            if (!_this.user.isAdmin) {
                this.userService.getSubordinateUsers().subscribe(users => _this.user.isSuperior = users != null && users.length > 0);
            }
        });
    }

    logout() {
        this.adalService.logout();
    }
}
