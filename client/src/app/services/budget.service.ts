import { Injectable } from '@angular/core';
import { Request } from '../model/request';
import { Observable ,  of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AdalService } from './adal.service';
import { User } from '../model/user';
import { Budget } from '../model/budget';

@Injectable()
export class BudgetService {

  url = "http://pbaserver.azurewebsites.net/api/Budget/user/current";

  constructor(private http: HttpClient, private adalService: AdalService) { }

  public getCurrentUsersBudgets(): Observable<Budget[]> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.get<Budget[]>(this.url, httpOptions)
  }
}