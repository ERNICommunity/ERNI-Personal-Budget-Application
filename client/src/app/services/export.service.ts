import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from './config.service';
import { isError } from 'util';
import { ServiceHelper } from './service.helper';
import { DownloadTokenService } from './download-token.service';

@Injectable()
export class ExportService {

  serviceUrl = 'export';

  constructor(private downloadTokenService: DownloadTokenService, private http: HttpClient, private serviceHelper: ServiceHelper, private configService: ConfigService) {
  }

  public async downloadExport(month: number, year: number) {
    const response = await this.downloadTokenService.getDownloadToken();
    if (!isError(response)) {
      window.location.href = `${this.configService.apiUrlBase}${this.serviceUrl}/requests/${response}/${year}/${month}`;
    }
  }
}
