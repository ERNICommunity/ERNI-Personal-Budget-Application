import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Budget } from '../model/budget';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';
import { BudgetType } from '../model/budgetType';
import { User } from '../model/user';
import { TeamBudget } from '../model/request/team-budget';

@Injectable()
export class BudgetService {

  url = "Budget/";

  constructor(private http: HttpClient, private serviceHelper: ServiceHelper, private configService: ConfigService) { }

  public getCurrentUsersBudgets(year: number): Observable<Budget[]> {
    return this.http.get<Budget[]>(this.configService.apiUrlBase + this.url + 'users/active/year/' + year, this.serviceHelper.getHttpOptions())
  }

  public getCurrentUserBudgets(year: number): Observable<Budget[]> {
    return this.http.get<Budget[]>(this.configService.apiUrlBase + this.url + 'user/current/year/' + year, this.serviceHelper.getHttpOptions())
  }

  public getTeamBudgets(year: number): Observable<TeamBudget> {
    return this.http.get<TeamBudget>(this.configService.apiUrlBase + this.url + 'user/team/year/' + year, this.serviceHelper.getHttpOptions());
  }

  public getUserBudgetByYear(userId, year: number): Observable<Budget> {
    return this.http.get<Budget>(this.configService.apiUrlBase + this.url + 'user/' + userId + '/year/' + year, this.serviceHelper.getHttpOptions())
  }

  public getBudgetsByYear(year: number): Observable<Budget[]> {
    return this.http.get<Budget[]>(this.configService.apiUrlBase + this.url + 'year/' + year, this.serviceHelper.getHttpOptions())
  }

  public updateBudget(budget: Budget): Observable<any> {
    return this.http.put(this.configService.apiUrlBase + this.url, budget, this.serviceHelper.getHttpOptions());
  }

  public createBudget(title: string, amount: number, userId: number, budgetType: number): Observable<any> {
    return this.http.post(this.configService.apiUrlBase + this.url + "users/" + userId, { title, amount, budgetType }, this.serviceHelper.getHttpOptions());
  }
  
  public createBudgetsForAllActiveUsers(title: string, amount: number, budgetType: number): Observable<any> {
    return this.http.post(this.configService.apiUrlBase + this.url + "users/all", { title, amount, budgetType }, this.serviceHelper.getHttpOptions());
  }

  public getBudgetsTypes(): Observable<BudgetType[]> {
    return this.http.get<BudgetType[]>(this.configService.apiUrlBase + this.url + 'types', this.serviceHelper.getHttpOptions());
  }

  public getUsersAvailableForBudgetType(budgetType: number): Observable<any> {
    return this.http.get<User>(this.configService.apiUrlBase + this.url + 'usersAvailableForBudgetType/' + budgetType, this.serviceHelper.getHttpOptions());
  }
}