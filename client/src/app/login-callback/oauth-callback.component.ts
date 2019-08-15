import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AdalService } from '../services/adal.service';
import { UserService } from '../services/user.service';

@Component({
    template: '<div>Authenticating user. Please wait...</div>'
})
export class OAuthCallbackComponent implements OnInit {
    constructor(private router: Router, private adalService: AdalService, private userService: UserService) {

    }

    ngOnInit() {
        if (!this.adalService.userInfo) {
            this.router.navigate(['login']);
        } else { 
            this.userService.registerUser().subscribe(u => this.router.navigate(['my-budget']));
        }
    }
}