import { Injectable } from '@angular/core';
import { Request } from '../model/request/request';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';
import { RequestMass } from '../model/requestMass';
import { BudgetLeft } from '../model/budgetLeft';
import { User } from '../model/user';
import { NewRequest } from '../model/newRequest';
import { PatchRequest } from '../model/PatchRequest';

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

  public getApprovedBySuperiorRequests(year: number): Observable<Request[]> {
    return this.http.get<Request[]>(this.configService.apiUrlBase + this.requestUrl + year + '/approvedBySuperior', this.serviceHelper.getHttpOptions())
  }

  public getRejectedRequests(year: number): Observable<Request[]> {
    return this.http.get<Request[]>(this.configService.apiUrlBase + this.requestUrl + year + '/rejected', this.serviceHelper.getHttpOptions())
  }

  public getRequests(budgetId: number): Observable<Request[]> {
    return this.http.get<Request[]>(this.configService.apiUrlBase + this.requestUrl + 'budget/' + budgetId, this.serviceHelper.getHttpOptions())
  }

  public getRequest(id): Observable<Request> {
    return this.http.get<Request>(this.configService.apiUrlBase + this.requestUrl + id, this.serviceHelper.getHttpOptions())
  }

  public approveRequest(id: number): Observable<Request> {
    return this.http.post<Request>(this.configService.apiUrlBase + this.requestUrl + id + '/approve', this.serviceHelper.getHttpOptions())
  }

  public rejectRequest(id: number): Observable<Request> {
    return this.http.post<Request>(this.configService.apiUrlBase + this.requestUrl + id + '/reject', this.serviceHelper.getHttpOptions())
  }

  public addRequest(request: NewRequest): Observable<any> {
    return this.http.post(this.configService.apiUrlBase + this.requestUrl, request, this.serviceHelper.getHttpOptions());
  }

  public addMassRequest(request: RequestMass): Observable<any> {
    return this.http.post<RequestMass>(this.configService.apiUrlBase + this.requestUrl + 'mass', request, this.serviceHelper.getHttpOptions());
  }

  public updateRequest(request: PatchRequest): Observable<any> {
    return this.http.put(this.configService.apiUrlBase + this.requestUrl, request, this.serviceHelper.getHttpOptions());
  }

  public deleteRequest(id: number): Observable<Request> {
    return this.http.delete<Request>(this.configService.apiUrlBase + this.requestUrl + id, this.serviceHelper.getHttpOptions());
  }

  public getUsersWithBudgetLeft(request: BudgetLeft): Observable<User[]> {
    return this.http.get<User[]>(this.configService.apiUrlBase + this.requestUrl + 'budget-left/' + request.amount +'/' + request.year, this.serviceHelper.getHttpOptions());
  }
}