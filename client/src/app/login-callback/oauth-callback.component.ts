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
            let sub = this.adalService.userInfo.profile['sub'];
            let firstName = this.adalService.userInfo.profile['given_name'];
            let lastName = this.adalService.userInfo.profile['family_name'];
            let upn = this.adalService.userInfo.profile['upn'];
            this.userService.registerUser({ sub, firstName, lastName, upn}).subscribe(u => this.router.navigate(['my-budget']));
        }
    }
}