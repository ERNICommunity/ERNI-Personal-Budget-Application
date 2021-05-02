import { Injectable } from "@angular/core";
import { Budget } from "../model/budget";
import { ServiceHelper } from "./service.helper";
import { HttpClient } from "@angular/common/http";
import { ConfigService } from "./config.service";
import { Observable } from "rxjs";
import { TeamBudgetModel } from "../model/teamBudget";
import { TeamRequestModel } from "../model/request/teamRequestModel";
import { NewTeamRequestModel } from "../model/request/newTeamRequestModel";

@Injectable()
export class TeamBudgetService {
  url = "TeamBudget/";

  constructor(
    private http: HttpClient,
    private serviceHelper: ServiceHelper,
    private configService: ConfigService
  ) {}

  public getCurrentUserBudgets(year: number): Observable<Budget[]> {
    return this.http.get<Budget[]>(
      this.configService.apiUrlBase + this.url + "user/current/year/" + year,
      this.serviceHelper.getHttpOptions()
    );
  }

  public getDefaultTeamBudget(year: number): Promise<TeamBudgetModel[]> {
    return this.http
      .get<TeamBudgetModel[]>(
        this.configService.apiUrlBase + this.url + "default-team/" + year,
        this.serviceHelper.getHttpOptions()
      )
      .toPromise();
  }

  public getAllTeamBudgets(year: number): Promise<TeamBudgetModel[]> {
    return this.http
      .get<TeamBudgetModel[]>(
        this.configService.apiUrlBase + this.url + "all-employees/" + year,
        this.serviceHelper.getHttpOptions()
      )
      .toPromise();
  }

  public getTeamRequests(year: number): Promise<TeamRequestModel[]> {
    return this.http
      .get<TeamRequestModel[]>(
        this.configService.apiUrlBase + this.url + "requests/" + year,
        this.serviceHelper.getHttpOptions()
      )
      .toPromise();
  }

  public createTeamRequest(model: NewTeamRequestModel): Promise<any> {
    return this.http
      .post(this.configService.apiUrlBase + this.url + "requests",
      model,
      this.serviceHelper.getHttpOptions())
      .toPromise();
  }
}
