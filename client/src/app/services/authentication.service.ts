import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { BehaviorSubject, filter, lastValueFrom, Observable } from 'rxjs';
import { UserInfo } from '../model/userInfo';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';
import { map } from 'rxjs/operators';
import { InteractionStatus } from '@azure/msal-browser';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private userInfoSubject = new BehaviorSubject<UserInfo | null>(null);
  public userInfo$ = this.userInfoSubject.asObservable();

  constructor(
    private http: HttpClient,
    private serviceHelper: ServiceHelper,
    private configService: ConfigService,
    private msalService: MsalService,
    private msalBroadcastService: MsalBroadcastService,
  ) {
    this.fetchUserInfo();
  }

  private fetchUserInfo(): void {
    this.http
      .get<{ name: string; roles: string[] }>(
        `${this.configService.apiUrlBase}Authorization/userInfo`,
        this.serviceHelper.getHttpOptions(),
      )
      .pipe(
        map((response) => ({
          name: response.name,
          isAdmin: response.roles.indexOf('PBA.Admin') >= 0,
          isFinance: response.roles.indexOf('PBA.Finance') >= 0,
          isSuperior: response.roles.indexOf('PBA.Superior') >= 0,
        })),
      )
      .subscribe((userInfo) => this.userInfoSubject.next(userInfo));
  }

  async logout(): Promise<void> {
    await lastValueFrom(
      this.msalService.logoutPopup({
        account: this.msalService.instance.getAllAccounts()[0],
        mainWindowRedirectUri: '/login',
      }),
    );

    this.userInfoSubject.next(null);
  }

  public isAuthenticated(): Observable<boolean> {
    return this.msalBroadcastService.inProgress$.pipe(
      filter((status: InteractionStatus) => status === InteractionStatus.None),
      map(() => this.msalService.instance.getAllAccounts().length > 0),
    );
  }
}
