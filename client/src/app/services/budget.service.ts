import { Injectable } from '@angular/core';
import { Request } from '../model/request';
import { Observable } from 'rxjs/Observable';
import { of } from 'rxjs/observable/of';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AdalService } from './adal.service';
import { User } from '../model/user';
import { Budget } from '../model/budget';

@Injectable()
export class BudgetService {

  url = "http://localhost:64246/api/Budget/user/current";

  constructor(private http: HttpClient, private adalService: AdalService) { }

  public getCurrentUsersBudgets(): Observable<Budget[]> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.get<Budget[]>(this.url, httpOptions)
  }
}