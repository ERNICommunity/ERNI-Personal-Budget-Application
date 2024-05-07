import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { CanActivate, UrlTree } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import { map } from 'rxjs/operators';

@Injectable()
export class SuperiorGuard implements CanActivate {
    constructor(private auth: AuthenticationService) {}

    canActivate():
        | Observable<boolean | UrlTree>
        | Promise<boolean | UrlTree>
        | boolean
        | UrlTree {
        return this.auth.userInfo$.pipe(map((_) => !!_ && _.isSuperior));
    }
}
