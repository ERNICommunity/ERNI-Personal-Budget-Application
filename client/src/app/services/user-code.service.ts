import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { User } from '../model/user';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';
import { switchMap } from 'rxjs/operators';
import { UserCode } from '../model/userCode';

@Injectable()
export class UserCodeService {

    url = "EmployeeCode";

    constructor(private http: HttpClient, private serviceHelper: ServiceHelper, private configService: ConfigService) {
    }

    public getUserCodes(): Observable<UserCode[]> {
        return this.http.get<UserCode[]>(this.configService.apiUrlBase + this.url, this.serviceHelper.getHttpOptions())
    }
}
