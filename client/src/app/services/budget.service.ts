import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Budget } from '../model/budget';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';

@Injectable()
export class BudgetService {

  url = "Budget/";

  constructor(private http: HttpClient, private serviceHelper: ServiceHelper, private configService: ConfigService) { }

  public getCurrentUsersBudgets(year : number): Observable<Budget[]> {
    return this.http.get<Budget[]>(this.configService.apiUrlBase + this.url +  'users/active/year/'+ year , this.serviceHelper.getHttpOptions())
  }

  public getCurrentUserBudget(year : number): Observable<Budget> {
    return this.http.get<Budget>(this.configService.apiUrlBase + this.url +  'user/current/year/'+ year , this.serviceHelper.getHttpOptions())
  }

  public getCurrentUserBudgetByYear(userId, year : number): Observable<Budget> {
    return this.http.get<Budget>(this.configService.apiUrlBase + this.url +  'user/'+ userId +'/year/'+ year , this.serviceHelper.getHttpOptions())
  }

  public getBudgetsByYear(year : number): Observable<Budget[]> {
    return this.http.get<Budget[]>(this.configService.apiUrlBase + this.url +  'year/'+ year , this.serviceHelper.getHttpOptions())
  }

  public updateBudget(budget: Budget): Observable<any> {
    return this.http.put(this.configService.apiUrlBase + this.url, budget, this.serviceHelper.getHttpOptions());
  }

  public setBudgetsForYear(budget: Budget): Observable<any> {
    return this.http.post(this.configService.apiUrlBase + this.url, budget, this.serviceHelper.getHttpOptions());
  }
}