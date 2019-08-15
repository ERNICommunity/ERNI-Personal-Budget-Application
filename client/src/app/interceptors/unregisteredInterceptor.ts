import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable } from "rxjs";
import { AdalService } from '../services/adal.service';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';

@Injectable()
export class UnregisteredInterceptor implements HttpInterceptor {

    constructor(private adalService: AdalService) {
    }

    private handleAuthError(err: HttpErrorResponse): Observable<any> {
        if (err.status === 401 || err.status === 403) {
            this.adalService.logout();
        }
        return Observable.throw(err);
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let authReq = req.clone({ headers: req.headers });
        return next.handle(authReq);
    }
}