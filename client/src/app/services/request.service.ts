import { Injectable } from "@angular/core";
import { Request } from "../model/request/request";
import { Observable, of } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { ConfigService } from "./config.service";
import { ServiceHelper } from "./service.helper";
import { MassRequest } from "../model/massRequest";
import { NewRequest } from "../model/newRequest";
import { PatchRequest } from "../model/PatchRequest";

@Injectable({
  providedIn: "root",
})
export class RequestService {
  requestUrl = "Request/";

  constructor(
    private http: HttpClient,
    private serviceHelper: ServiceHelper,
    private configService: ConfigService
  ) {}

  public getRequests(
    year: number,
    state: "approved" | "rejected" | "pending",
    budgetTypeId: number
  ): Observable<Request[]> {
    return this.http.get<Request[]>(
      this.configService.apiUrlBase +
        this.requestUrl +
        year +
        `/state/${state}/type/` +
        budgetTypeId,
      this.serviceHelper.getHttpOptions()
    );
  }

  public getRequest(id: number): Observable<Request> {
    return this.http.get<Request>(
      this.configService.apiUrlBase + this.requestUrl + id,
      this.serviceHelper.getHttpOptions()
    );
  }

  public approveRequest(id: number): Observable<Request> {
    return this.http.post<Request>(
      this.configService.apiUrlBase + this.requestUrl + id + "/approve",
      this.serviceHelper.getHttpOptions()
    );
  }

  public rejectRequest(id: number): Observable<Request> {
    return this.http.post<Request>(
      this.configService.apiUrlBase + this.requestUrl + id + "/reject",
      this.serviceHelper.getHttpOptions()
    );
  }

  public addRequest(request: NewRequest): Observable<number> {
    return this.http.post<number>(
      this.configService.apiUrlBase + this.requestUrl,
      request,
      this.serviceHelper.getHttpOptions()
    );
  }

  public addTeamRequest(request: NewRequest): Observable<number> {
    return this.http.post<number>(
      this.configService.apiUrlBase + this.requestUrl + "team",
      request,
      this.serviceHelper.getHttpOptions()
    );
  }

  public addMassRequest(request: MassRequest): Observable<any> {
    return this.http.post<MassRequest>(
      this.configService.apiUrlBase + this.requestUrl + "mass",
      request,
      this.serviceHelper.getHttpOptions()
    );
  }

  public updateRequest(request: PatchRequest): Observable<any> {
    return this.http.put(
      this.configService.apiUrlBase + this.requestUrl,
      request,
      this.serviceHelper.getHttpOptions()
    );
  }

  public updateTeamRequest(request: PatchRequest): Observable<any> {
    return this.http.put(
      this.configService.apiUrlBase + this.requestUrl + "team",
      request,
      this.serviceHelper.getHttpOptions()
    );
  }

  public deleteRequest(id: number): Observable<any> {
    return this.http.delete<Request>(
      this.configService.apiUrlBase + this.requestUrl + id,
      this.serviceHelper.getHttpOptions()
    );
  }

  public getRemainingBudgets(): Observable<
    {
      id: number;
      firstName: string;
      lastName: string;
      budgetLeft: number;
    }[]
  > {
    return this.http.get<
      {
        id: number;
        firstName: string;
        lastName: string;
        budgetLeft: number;
      }[]
    >(
      this.configService.apiUrlBase + this.requestUrl + "personal-budget-left",
      this.serviceHelper.getHttpOptions()
    );
  }
}
