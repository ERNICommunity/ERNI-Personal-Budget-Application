import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { User } from '../model/user';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';
import { switchMap } from 'rxjs/operators';

@Injectable()
export class UserService {
    private currentUser: string = "currentUser";

    url = "User";

    constructor(private http: HttpClient, private serviceHelper: ServiceHelper, private configService: ConfigService) {
    }

    public createUser(data) {
        return this.http.post(this.configService.apiUrlBase + this.url + '/create', data, this.serviceHelper.getHttpOptions());
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

    public updateUser(user: User): Observable<any> {
        return this.http.put(this.configService.apiUrlBase + this.url, user, this.serviceHelper.getHttpOptions());
    }

    public addUser(user: User): Observable<User> {
        return this.http.post<User>(this.configService.apiUrlBase + this.url, user, this.serviceHelper.getHttpOptions());
    }

    public synchronizeUsers(): Observable<any> {
        return this.http.get(this.configService.apiUrlBase + this.url + '/synchronize', this.serviceHelper.getHttpOptions());
    }
}
