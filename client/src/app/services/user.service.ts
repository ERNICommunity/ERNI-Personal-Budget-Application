import { Injectable } from '@angular/core';
import { Request } from '../model/request';
import { Observable } from 'rxjs/Observable';
import { of } from 'rxjs/observable/of';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AdalService } from './adal.service';
import { User } from '../model/user';

@Injectable()
export class UserService {

  url = "http://localhost:64246/api/User";

  constructor(private http: HttpClient, private adalService: AdalService) { }

  public getRequests(): Observable<User[]> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.get<User[]>(this.url, httpOptions)
  }
}