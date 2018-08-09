import { Injectable } from '@angular/core';
import { Request } from '../model/request';
import { Observable ,  of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AdalService } from './adal.service';
import { ConfigService } from './config.service';

@Injectable()
export class RequestService {

  requestUrl = "Request/";

  constructor(private http: HttpClient, private adalService: AdalService, private configService: ConfigService) { }


  public getPendingRequests(): Observable<Request[]> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json',
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.get<Request[]>(this.configService.apiUrlBase + this.requestUrl + '/pending', httpOptions)
  }

  public getRequests(year): Observable<Request[]> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json',
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.get<Request[]>(this.configService.apiUrlBase + this.requestUrl + 'user/current/year/' + year, httpOptions)
  }
}