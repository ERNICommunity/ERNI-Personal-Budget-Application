import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

import { AdalService } from '../services/adal.service';
import { UserService } from '../services/user.service';

@Injectable()
export class OAuthCallbackHandler implements CanActivate {
    constructor(private router: Router, private adalService: AdalService, private userService: UserService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        this.adalService.handleCallback();

        if (this.adalService.userInfo) {
            return true;
        }

        return false;
    }
}
