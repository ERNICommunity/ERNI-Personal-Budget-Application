import { Injectable } from '@angular/core';
import { Request } from '../model/request';
import { Observable ,  of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AdalService } from './adal.service';
import { User } from '../model/user';
import { Budget } from '../model/budget';
import { ConfigService } from './config.service';

@Injectable()
export class BudgetService {

  url = "Budget/user/current";

  constructor(private http: HttpClient, private adalService: AdalService, private configService: ConfigService) { }

  public getCurrentUsersBudgets(): Observable<Budget[]> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.get<Budget[]>(this.configService.apiUrlBase + this.url, httpOptions)
  }
}