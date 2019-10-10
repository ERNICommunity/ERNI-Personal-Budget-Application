import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthenticationGuard } from './authentication.guard';
import { AdalService } from '../adal.service';
import { UserService } from '../user.service';

@Injectable()
export class AdminGuard extends AuthenticationGuard {

    constructor(protected router: Router, protected adalService: AdalService, protected userService: UserService) {
        super(router, adalService, userService);
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
        if(super.canActivate(route, state))
        {
            return this.user.isAdmin;
        }
        return false;
    }
}
