import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { MsalService } from '@azure/msal-angular';
import { lastValueFrom, Observable, shareReplay } from 'rxjs';
import { UserInfo } from '../model/userInfo';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  public userInfo$: Observable<UserInfo>;

  constructor(
    private http: HttpClient,
    private serviceHelper: ServiceHelper,
    private configService: ConfigService,
    private msalService: MsalService,
  ) {
    this.userInfo$ = this.getUserInfo().pipe(
      map((response) => ({
        name: response.name,
        isAdmin: response.roles.indexOf('PBA.Admin') >= 0,
        isFinance: response.roles.indexOf('PBA.Finance') >= 0,
        isSuperior: response.roles.indexOf('PBA.Superior') >= 0,
      })),
      shareReplay(1),
    );
  }

  private getUserInfo(): Observable<{ name: string; roles: string[] }> {
    return this.http.get<{ name: string; roles: string[] }>(
      `${this.configService.apiUrlBase}Authorization/userInfo`,
      this.serviceHelper.getHttpOptions(),
    );
  }

  async logout(): Promise<void> {
    await lastValueFrom(this.msalService.logoutPopup());
  }
}
