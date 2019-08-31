import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { User } from '../model/user';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';
import { setTime } from 'ngx-bootstrap/chronos/utils/date-setters';

@Injectable()
export class UserService {

    url = "User";

    constructor(private http: HttpClient, private serviceHelper: ServiceHelper, private configService: ConfigService) {
    }

    public registerUser(): Observable<any> {
        return this.http.post<any>(this.configService.apiUrlBase + this.url + '/register', this.serviceHelper.getHttpOptions());
    }

    public getActiveUsers(): Observable<User[]> {
        return this.http.get<User[]>(this.configService.apiUrlBase + this.url + '/active', this.serviceHelper.getHttpOptions())
    }

    public getSubordinateUsers(): Observable<User[]> {
        return this.http.get<User[]>(this.configService.apiUrlBase + this.url, this.serviceHelper.getHttpOptions())
    }

    public getUser(id): Observable<User> {
        return this.http.get<User>(this.configService.apiUrlBase + this.url + "/" + id, this.serviceHelper.getHttpOptions())
    }

    public getCurrentUser(): Observable<User> {
        let json = localStorage.getItem('currentUser');
        if (json) {
            let user = JSON.parse(json);
            return new Observable<User>(
                observer => {
                    setTimeout(() => {
                        observer.next(user);
                    }, 1000);
                });
        }
        else
        {
            let observable = this.http.get<User>(this.configService.apiUrlBase + this.url + "/current", this.serviceHelper.getHttpOptions())
            observable.subscribe(u => localStorage.setItem('currentUser', JSON.stringify(u)));
            return observable;
        }
    }

    public updateUser(user: User): Observable<any> {
        return this.http.put(this.configService.apiUrlBase + this.url, user, this.serviceHelper.getHttpOptions());
    }

    public addUser(user: User): Observable<User> {
        return this.http.post<User>(this.configService.apiUrlBase + this.url, user, this.serviceHelper.getHttpOptions());
    }
}
