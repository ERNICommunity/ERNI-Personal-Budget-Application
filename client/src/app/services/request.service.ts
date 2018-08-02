import { Injectable } from '@angular/core';
import { Request } from '../model/request';
import { Observable ,  of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AdalService } from './adal.service';

@Injectable()
export class RequestService {

  requestUrl = "http://pbaserver.azurewebsites.net/api/Request/";

  constructor(private http: HttpClient, private adalService: AdalService) { }


  public getPendingRequests(): Observable<Request[]> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json',
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.get<Request[]>(this.requestUrl + '/pending', httpOptions)
  }

  public getRequests(year): Observable<Request[]> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json',
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.get<Request[]>(this.requestUrl + 'user/current/year/' + year, httpOptions)
  }
}