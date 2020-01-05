import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationGuard } from './authentication.guard';

@Injectable()
export class ViewerGuard extends AuthenticationGuard {

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

        if(super.canActivate)
        {
            return this.user.isViewer || this.user.isAdmin;
        }
        return false;
    }
}
