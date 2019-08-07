import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AdalService } from '../services/adal.service';
import { UserService } from '../services/user.service';
import { RegisterUser } from '../model/registerUser';
import { User } from '../model/user';

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
            var user: User;
            var reg: RegisterUser = new RegisterUser();
            reg.sub = this.adalService.userInfo.profile['sub'];
            reg.firstName = this.adalService.userInfo.profile['given_name'];
            reg.lastName = this.adalService.userInfo.profile['family_name'];
            reg.userName = this.adalService.userInfo.profile['upn'];
            this.userService.registerUser(reg).subscribe(u => this.router.navigate(['my-budget']));
        }
    }
}