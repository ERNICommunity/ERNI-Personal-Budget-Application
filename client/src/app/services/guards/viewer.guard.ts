import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ViewerGuard implements CanActivate {
  constructor(private auth: AuthenticationService) {}

  canActivate(
    _route: ActivatedRouteSnapshot,
    _state: RouterStateSnapshot,
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this.auth.userInfo$.pipe(map((_) => !!_ && (_.isAdmin || _.isFinance)));
  }
}
