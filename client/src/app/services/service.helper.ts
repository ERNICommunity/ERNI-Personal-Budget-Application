import { Injectable } from "../../../node_modules/@angular/core";
import { AdalService } from "./adal.service";
import { HttpHeaders, HttpParams } from "../../../node_modules/@angular/common/http";

@Injectable()
export class ServiceHelper {
    constructor(private adalService: AdalService) {
    }

    public getHttpOptions(): {
        headers?: HttpHeaders | {
            [header: string]: string | string[];
        };
        observe?: "body";
        params?: HttpParams | {
            [param: string]: string | string[];
        };
        reportProgress?: boolean;
        responseType?: "json";
        withCredentials?: boolean;
    } {
        return {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.adalService.accessToken
            })
        };
    }
}