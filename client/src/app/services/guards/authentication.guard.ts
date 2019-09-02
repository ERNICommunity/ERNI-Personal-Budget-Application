import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, NavigationExtras } from '@angular/router';
import { AdalService } from '../adal.service';
import { UserService } from '../user.service';
import { User } from '../../model/user';

@Injectable()
export class AuthenticationGuard implements CanActivate {
    protected user: User;

    constructor(protected router: Router, protected adalService: AdalService, protected userService: UserService) {
        this.user = this.userService.getCurrentUserStorage();
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

        let navigationExtras: NavigationExtras = { };

        if (!this.adalService.isAuthenticated) {
            this.router.navigate(['login'], navigationExtras);
            return false;
        }

        return true;
    }
}
