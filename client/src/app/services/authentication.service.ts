import { Injectable } from '@angular/core';

import { MsalService, MsalBroadcastService } from '@azure/msal-angular';
import { EventMessage, AuthenticationResult, EventType } from '@azure/msal-browser';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserInfo } from '../model/userInfo';

@Injectable()
export class AuthenticationService {
  private sessionStorageKey = 'rmt-currentUser';

  private userInfoSubject: BehaviorSubject<UserInfo>;
  userInfo$: Observable<UserInfo>;

  constructor(
    private msalService: MsalService,
    private msalBroadcast: MsalBroadcastService) {
      const currentUserJson = sessionStorage.getItem(this.sessionStorageKey);
      if (currentUserJson) {
        this.userInfoSubject = new BehaviorSubject<UserInfo>(JSON.parse(currentUserJson));
      } else {
        this.userInfoSubject = new BehaviorSubject<UserInfo>(undefined);
      }

      this.userInfo$ = this.userInfoSubject.asObservable();
      this.msalBroadcast.msalSubject$.subscribe(_ => this.handleMsalMessage(_));
    }

  get isAuthenticated(): boolean {
    return this.userInfoSubject.value !== undefined;
  }

  get userInfo(): UserInfo {
    return this.userInfoSubject.value;
  }

  async login(): Promise<boolean> {
    try {
      await this.msalService.loginPopup().toPromise();
      return true;
    } catch (error) {
      return false;
    }
  }

  async logout() {
    sessionStorage.removeItem(this.sessionStorageKey);
    this.userInfoSubject.next(undefined);
    await this.msalService.logout().toPromise();
  }

  private handleMsalMessage(message: EventMessage) {
    if (message.eventType !== EventType.ACQUIRE_TOKEN_SUCCESS && message.eventType !== EventType.LOGIN_SUCCESS) {
      return;
    }

    const result = message.payload as AuthenticationResult;

    // the app will get several types of tokens. the token that signifies user's login into the app
    // has 'openid' scope. The others are scoped for the API and we are not interested in those
    if (result.scopes.indexOf('openid') === -1) {
      return;
    }

    const userInfo = this.parseUser(result);

    sessionStorage.setItem(this.sessionStorageKey, JSON.stringify(userInfo));

    this.userInfoSubject.next(userInfo);
  }

  private parseUser(authResult: AuthenticationResult): UserInfo {
    const name = authResult.idTokenClaims['name'] as string;
    const roles = authResult.idTokenClaims['roles'] as string[];

    return { isAdmin: roles.indexOf('PBA.Admin') >= 0, isUser: roles.indexOf('PBA.Employee') >= 0, isFinance: roles.indexOf('PBA.Finance') >= 0, name };
  }
}
