import { Injectable } from '@angular/core';
import { Budget } from '../model/budget';
import { ServiceHelper } from './service.helper';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from './config.service';
import { firstValueFrom } from 'rxjs';
import { TeamBudgetModel } from '../model/teamBudget';
import { TeamRequestModel } from '../model/request/teamRequestModel';
import { NewTeamRequestModel } from '../model/request/newTeamRequestModel';

@Injectable({ providedIn: 'root' })
export class TeamBudgetService {
  url = 'TeamBudget/';

  constructor(
    private http: HttpClient,
    private serviceHelper: ServiceHelper,
    private configService: ConfigService,
  ) {}

  public getCurrentUserBudgets(year: number) {
    return this.http.get<Budget[]>(
      this.configService.apiUrlBase + this.url + 'user/current/year/' + year,
      this.serviceHelper.getHttpOptions(),
    );
  }

  public getDefaultTeamBudget(year: number) {
    return firstValueFrom(
      this.http.get<TeamBudgetModel[]>(
        this.configService.apiUrlBase + this.url + 'default-team/' + year,
        this.serviceHelper.getHttpOptions(),
      ),
    );
  }

  public getTeamRequests(year: number) {
    return firstValueFrom(
      this.http.get<TeamRequestModel[]>(
        this.configService.apiUrlBase + this.url + 'requests/' + year,
        this.serviceHelper.getHttpOptions(),
      ),
    );
  }

  public getSingleTeamRequest(requestId: number) {
    return firstValueFrom(
      this.http.get<TeamRequestModel>(
        this.configService.apiUrlBase + this.url + 'request/' + requestId,
        this.serviceHelper.getHttpOptions(),
      ),
    );
  }

  public createTeamRequest(model: NewTeamRequestModel) {
    return firstValueFrom(
      this.http.post<number>(
        this.configService.apiUrlBase + this.url + 'requests',
        model,
        this.serviceHelper.getHttpOptions(),
      ),
    );
  }

  public updateTeamRequest(requestId: number, model: NewTeamRequestModel) {
    return firstValueFrom(
      this.http.patch<void>(
        this.configService.apiUrlBase + this.url + 'request/' + requestId,
        model,
        this.serviceHelper.getHttpOptions(),
      ),
    );
  }
}
