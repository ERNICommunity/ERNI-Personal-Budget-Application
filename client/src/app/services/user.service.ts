import { Injectable } from '@angular/core';
import { Request } from '../model/request';
import { Observable ,  of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AdalService } from './adal.service';
import { User } from '../model/user';

@Injectable()
export class UserService {

  url = "http://localhost:64246/api/User";

  constructor(private http: HttpClient, private adalService: AdalService) { }

  public getRequests(): Observable<User[]> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.get<User[]>(this.url, httpOptions)
  }

  public getUser(id): Observable<User> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.get<User>(this.url + "/" + id, httpOptions)
  }

  public updateUser(user: User): Observable<any> {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 
      'Authorization': 'Bearer ' +  this.adalService.accessToken })
    };

    return this.http.put(this.url, user, httpOptions);/*.pipe(
      tap(_ => this.log(`updated hero id=${hero.id}`)),
      catchError(this.handleError<any>('updateHero'))
    );*/
  }
    public addUser (user: User): Observable<User> {
      var httpOptions = {
        headers: new HttpHeaders({ 'Content-Type': 'application/json', 
        'Authorization': 'Bearer ' +  this.adalService.accessToken })
      };

      return this.http.post<User>(this.url, user, httpOptions);/*.pipe(
        tap((hero: Hero) => this.log(`added hero w/ id=${hero.id}`)),
        catchError(this.handleError<Hero>('addHero'))
      );*/
    }
  }
