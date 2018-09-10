import {Injectable} from '@angular/core';
import {HttpEvent, HttpInterceptor, HttpHandler, HttpRequest} from '@angular/common/http';
import {Observable} from "rxjs";
import { AdalService } from '../services/adal.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  
    constructor(private adalService: AdalService) {
    }
  
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    req = req.clone({
      setHeaders: {
        Authorization: 'Bearer ' + this.adalService.accessToken
      }
    });

    return next.handle(req);
  }
}