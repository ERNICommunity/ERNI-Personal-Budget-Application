import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { User } from '../model/user';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';
import { UserUpdateModel } from '../model/userUpdateModel';

@Injectable({ providedIn: 'root' })
export class UserService {
  url = 'User';

  constructor(
    private http: HttpClient,
    private serviceHelper: ServiceHelper,
    private configService: ConfigService,
  ) {}

  public createUser(data: { firstName: string; lastName: string; email: string; superior: number; state: number }) {
    return this.http.post(
      this.configService.apiUrlBase + this.url + '/create',
      data,
      this.serviceHelper.getHttpOptions(),
    );
  }

  public getActiveUsers(): Observable<User[]> {
    return this.http.get<User[]>(
      this.configService.apiUrlBase + this.url + '/active',
      this.serviceHelper.getHttpOptions(),
    );
  }

  public getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.configService.apiUrlBase + this.url, this.serviceHelper.getHttpOptions());
  }

  public syncUsers() {
    return this.http.post(
      this.configService.apiUrlBase + this.url + '/sync',
      null,
      this.serviceHelper.getHttpOptions(),
    );
  }

  public getUser(id: number): Observable<User> {
    return this.http.get<User>(
      this.configService.apiUrlBase + this.url + '/' + id,
      this.serviceHelper.getHttpOptions(),
    );
  }

  public updateUser(user: UserUpdateModel) {
    return this.http.put<void>(this.configService.apiUrlBase + this.url, user, this.serviceHelper.getHttpOptions());
  }

  public activateUser(id: number) {
    return this.http.post<void>(
      `${this.configService.apiUrlBase}${this.url}/${id}/activate`,
      this.serviceHelper.getHttpOptions(),
    );
  }

  public deactivateUser(id: number) {
    return this.http.post<void>(
      `${this.configService.apiUrlBase}${this.url}/${id}/deactivate`,
      this.serviceHelper.getHttpOptions(),
    );
  }

  public addUser(user: User): Observable<User> {
    return this.http.post<User>(this.configService.apiUrlBase + this.url, user, this.serviceHelper.getHttpOptions());
  }
}
