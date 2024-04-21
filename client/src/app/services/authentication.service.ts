import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { MsalService } from '@azure/msal-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserInfo } from '../model/userInfo';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    url = 'Authorization/';

    public userInfo$ = new BehaviorSubject<UserInfo>(null);

    constructor(
        private http: HttpClient,
        private serviceHelper: ServiceHelper,
        private configService: ConfigService,
        private msalService: MsalService
    ) {
        this.getUserInfo().subscribe((_) => {
            this.userInfo$.next({
                name: _.name,
                isAdmin: _.roles.indexOf('PBA.Admin') >= 0,
                isFinance: _.roles.indexOf('PBA.Finance') >= 0,
                isSuperior: _.roles.indexOf('PBA.Superior') >= 0
            });
        });
    }

    private getUserInfo(): Observable<{ name: string; roles: string[] }> {
        return this.http.get<{ name: string; roles: string[] }>(
            this.configService.apiUrlBase + this.url + 'userInfo',
            this.serviceHelper.getHttpOptions()
        );
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
        await this.msalService.logout().toPromise();
    }
}
