import { Component } from '@angular/core';
import { BusyIndicatorService } from './services/busy-indicator.service';
import { AuthenticationService } from './services/authentication.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {
    constructor(
        public authService: AuthenticationService,
        public busyIndicatorService: BusyIndicatorService
    ) {}

    async logout(): Promise<void> {
        await this.authService.logout();
    }
}
