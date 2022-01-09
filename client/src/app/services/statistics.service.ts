import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { ConfigService } from "./config.service";
import { isError } from "util";
import { ServiceHelper } from "./service.helper";
import { StatisticsModel } from "../model/statisticsModel";

@Injectable()
export class StatisticsService {
  serviceUrl = "statistics";

  constructor(
    private http: HttpClient,
    private serviceHelper: ServiceHelper,
    private configService: ConfigService
  ) {}

  public getStatistics(year: number): Promise<StatisticsModel> {
    // return new Promise(() => {
    //   budgets: [
    //     {
    //       budgetType: 0,
    //       totalAmount: 23090,
    //       totalSpentAmount: 15389,
    //     },
    //   ];
    // });

    return this.http.get<StatisticsModel>(`${this.configService.apiUrlBase}${this.serviceUrl}/${year}`, this.serviceHelper.getHttpOptions()).toPromise();
  }
}
