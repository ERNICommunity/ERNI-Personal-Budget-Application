import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import { map } from 'rxjs/operators';

@Injectable()
export class ViewerGuard  {
    constructor(private auth: AuthenticationService) {}

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ):
        | Observable<boolean | UrlTree>
        | Promise<boolean | UrlTree>
        | boolean
        | UrlTree {
        return this.auth.userInfo$.pipe(
            map((_) => !!_ && (_.isAdmin || _.isFinance))
        );
    }
}
