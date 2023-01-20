import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MsalService } from '@azure/msal-angular';
import { lastValueFrom } from 'rxjs';

@Component({
    selector: 'rmt-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent {
    constructor(
        private readonly msalService: MsalService,
        private readonly router: Router
    ) {}

    async login(): Promise<void> {
        await lastValueFrom(this.msalService.loginPopup());
        this.router.navigate(['my-budget']);
    }
}
