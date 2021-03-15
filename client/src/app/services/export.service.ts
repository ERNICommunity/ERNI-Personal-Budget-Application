import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from './config.service';
import { isError } from 'util';
import { ServiceHelper } from './service.helper';

@Injectable()
export class ExportService {

  serviceUrl = 'export';

  constructor(private http: HttpClient, private serviceHelper: ServiceHelper, private configService: ConfigService) {
  }

  private getDownloadToken(): Promise<any> {
    return this.http.get<any>(`${this.configService.apiUrlBase}${this.serviceUrl}/token`, this.serviceHelper.getHttpOptions()).toPromise();
  }

  public async downloadExport(month: number, year: number) {
    const response = await this.getDownloadToken();
    if (!isError(response)) {
      window.location.href = `${this.configService.apiUrlBase}${this.serviceUrl}/requests/${response}/${year}/${month}`;
    }
  }
}
