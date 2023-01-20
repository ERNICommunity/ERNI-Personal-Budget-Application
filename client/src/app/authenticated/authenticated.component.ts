import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';

@Component({
    selector: 'authenticated',
    templateUrl: './authenticated.component.html',
    styleUrls: ['./authenticated.component.scss']
})
export class AuthenticatedComponent implements OnInit {
    constructor(
        public authService: AuthenticationService,
        public router: Router
    ) {}

    ngOnInit(): void {}

    async logout(): Promise<void> {
        await this.authService.logout();
        this.router.navigate(['/login']);
    }
}
