import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AdalService } from '../services/adal.service';
import { UserService } from '../services/user.service';
import { HttpErrorResponse } from '@angular/common/http';
import { AlertService } from '../services/alert.service';
import { User } from '../model/user';

@Component({
    templateUrl: './oauth-callback.component.html'
})
export class OAuthCallbackComponent implements OnInit {
    loading: boolean;

    constructor(
        private router: Router,
        private adalService: AdalService,
        private userService: UserService,
        private alertService: AlertService) { }

    ngOnInit() {
        this.loading = true;

        if (!this.adalService.userInfo) {
            this.router.navigate(['/login']);
        } else {
            this.userService.registerUser().subscribe(
                (user: User) => {
                    this.userService.setCurrentUser(user);
                    this.router.navigate(['/my-budget']);
                },
                (err: HttpErrorResponse) => {
                    let error;
                    if (err.status == 403) {
                        error = "Your user account is not activated in PBA, please contact HR department!";
                    } else {
                        error = "Error occurred, please contact HR department!";
                    }

                    sessionStorage.clear();
                    this.alertService.error(error);
                    this.router.navigate(['/login']);
                })
                .add(() => this.loading = false);
        }
    }
}