import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthenticationService } from '../authentication.service';

@Injectable()
export class ViewerGuard implements CanActivate {
    constructor(private auth: AuthenticationService) {}
  
    canActivate(
      route: ActivatedRouteSnapshot,
      state: RouterStateSnapshot
    ): Observable<boolean|UrlTree>|Promise<boolean|UrlTree>|boolean|UrlTree {
      const userInfo = this.auth.userInfo;
  
      if (!userInfo) {
        return false;
      }
  
      return userInfo.isUser || userInfo.isAdmin || userInfo.isFinance;
    }
  }
 