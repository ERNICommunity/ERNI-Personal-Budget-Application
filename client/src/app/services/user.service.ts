import {Injectable} from '@angular/core';
import {Request} from '../model/request';
import {Observable, of} from 'rxjs';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {AdalService} from './adal.service';
import {User} from '../model/user';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';

@Injectable()
export class UserService {

    url = "User";

    constructor(private http: HttpClient, private serviceHelper: ServiceHelper, private configService: ConfigService) {
    }

    public getRequests(): Observable<User[]> {
        return this.http.get<User[]>(this.configService.apiUrlBase + this.url, this.serviceHelper.getHttpOptions())
    }

    public getUser(id): Observable<User> {
        return this.http.get<User>(this.configService.apiUrlBase + this.url + "/" + id, this.serviceHelper.getHttpOptions())
    }

    public getCurrentUser(): Observable<User> {
        return this.http.get<User>(this.configService.apiUrlBase + this.url + "/current", this.serviceHelper.getHttpOptions())
    }

    public updateUser(user: User): Observable<any> {
        return this.http.put(this.configService.apiUrlBase + this.url, user, this.serviceHelper.getHttpOptions());
        /*.pipe(
         tap(_ => this.log(`updated hero id=${hero.id}`)),
         catchError(this.handleError<any>('updateHero'))
         );*/
    }

    public addUser(user: User): Observable<User> {
        return this.http.post<User>(this.configService.apiUrlBase + this.url, user, this.serviceHelper.getHttpOptions());
        /*.pipe(
         tap((hero: Hero) => this.log(`added hero w/ id=${hero.id}`)),
         catchError(this.handleError<Hero>('addHero'))
         );*/
    }
}
