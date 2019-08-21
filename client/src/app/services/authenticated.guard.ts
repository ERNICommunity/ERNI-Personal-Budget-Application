import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, NavigationExtras } from '@angular/router';
import { AdalService } from './adal.service';
import { UserService } from './user.service';
import { User } from '../model/user';

@Injectable()
export class AuthenticationGuard implements CanActivate {
    user: User;
    isSuperior: boolean;

    constructor(private router: Router, private adalService: AdalService, private userService: UserService) {
        this.userService.getCurrentUser().subscribe(u => {
            this.user = u;
            this.userService.getSubordinateUsers().subscribe(users => this.isSuperior = users != null && users.length > 0);
        });
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

        let navigationExtras: NavigationExtras = {
            //queryParams: { 'redirectUrl': route.url }
        };

        if (!this.adalService.isAuthenticated) {
            this.router.navigate(['login'], navigationExtras);
            return false;
        }

        if (this.user) {
            if (route.url[0].path == 'other-budgets' || route.url[0].path == 'users' || route.url[0].path == 'categories' || route.url[0].path == 'request-mass') {
                if (!this.user.isAdmin) {
                    return false;
                }
            }

            if (route.url[0].path == 'requests') {
                if (!this.user.isAdmin && !this.isSuperior && !this.user.isViewer) {
                    return false;
                }
            }
        }
        return true;
    }
}
