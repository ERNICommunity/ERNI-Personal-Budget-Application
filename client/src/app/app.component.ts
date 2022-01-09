import { Component, DoCheck, OnInit } from '@angular/core';
import { UserService } from './services/user.service';
import { Router, NavigationStart, NavigationCancel, NavigationError, NavigationEnd } from '@angular/router';
import { BusyIndicatorService } from './services/busy-indicator.service';
import { User } from './model/user';
import { AuthenticationService } from './services/authentication.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {
    user: User;
    initialized: boolean;

    constructor(public authService: AuthenticationService, private userService: UserService, private router: Router, public busyIndicatorService: BusyIndicatorService) {
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

    async logout(): Promise<void> {
        await this.authService.logout();
    }
}