import { Injectable } from '@angular/core';
import { Request } from '../model/request';
import { Observable ,  of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AdalService } from './adal.service';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';

@Injectable()
export class RequestService {

  requestUrl = "Request/";

  constructor(private http: HttpClient, private serviceHelper: ServiceHelper, private configService: ConfigService) { }


  public getPendingRequests(year: number): Observable<Request[]> {
    return this.http.get<Request[]>(this.configService.apiUrlBase + this.requestUrl + year + '/pending', this.serviceHelper.getHttpOptions())
  }

  public getApprovedRequests(year: number): Observable<Request[]> {
    return this.http.get<Request[]>(this.configService.apiUrlBase + this.requestUrl + year + '/approved', this.serviceHelper.getHttpOptions())
  }

  public getRejectedRequests(year: number): Observable<Request[]> {
    return this.http.get<Request[]>(this.configService.apiUrlBase + this.requestUrl + year + '/rejected', this.serviceHelper.getHttpOptions())
  }

  public getRequests(year): Observable<Request[]> {
    return this.http.get<Request[]>(this.configService.apiUrlBase + this.requestUrl + 'user/current/year/' + year, this.serviceHelper.getHttpOptions())
  }

  public approveRequest(id: number): Observable<Request> {
    return this.http.post<Request>(this.configService.apiUrlBase + this.requestUrl + id + '/approve', this.serviceHelper.getHttpOptions())
  }

  public rejectRequest(id: number): Observable<Request> {
    return this.http.post<Request>(this.configService.apiUrlBase + this.requestUrl + id + '/reject', this.serviceHelper.getHttpOptions())
  }
}