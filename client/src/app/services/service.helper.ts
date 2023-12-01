import { Injectable } from "@angular/core";
import { HttpHeaders, HttpParams } from "@angular/common/http";

@Injectable({
  providedIn: "root",
})
export class ServiceHelper {
  constructor() {}

  public getHttpOptions(): {
    headers?:
      | HttpHeaders
      | {
          [header: string]: string | string[];
        };
    observe?: "body";
    params?:
      | HttpParams
      | {
          [param: string]: string | string[];
        };
    reportProgress?: boolean;
    responseType?: "json";
    withCredentials?: boolean;
  } {
    return {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
      }),
    };
  }
}
