import { Injectable } from '@angular/core';
import { Request } from '../model/request';
import { Observable ,  of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AdalService } from './adal.service';

@Injectable()
export class RequestService {

  heroesUrl = "http://localhost:64246/api/Request/user/current/year/";

  constructor(private http: HttpClient, private adalService: AdalService) { }

  public getRequests(year: number): Observable<Request[]> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.get<Request[]>(this.heroesUrl + year, httpOptions)
  }
}