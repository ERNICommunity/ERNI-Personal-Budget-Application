import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpResponse, HttpHeaders } from '@angular/common/http';
import { ConfigService } from './config.service';
import { Observable, firstValueFrom, lastValueFrom } from 'rxjs';
import { ServiceHelper } from './service.helper';
import { InvoiceImage } from '../model/InvoiceImage';
import { map } from 'rxjs/operators';

export type InvoiceUploadStatus = InvoiceUploadProgress | InvoiceUploadCompleted;
export interface InvoiceUploadProgress {
  status: 'in-progress';
  progress: number;
}
export interface InvoiceUploadCompleted {
  status: 'completed';
  id: number;
}

@Injectable({ providedIn: 'root' })
export class InvoiceImageService {
  requestUrl = 'InvoiceImage';

  constructor(
    private http: HttpClient,
    private configService: ConfigService,
    private serviceHelper: ServiceHelper,
  ) {}

  public getInvoiceImages(requestId: number): Observable<{ id: number; name: string }[]> {
    return this.http.get<{ id: number; name: string }[]>(
      this.configService.apiUrlBase + this.requestUrl + '/images/' + requestId,
      this.serviceHelper.getHttpOptions(),
    );
  }

  public getDownloadToken(imageId: number): Promise<string> {
    return firstValueFrom(
      this.http.get<string>(
        `${this.configService.apiUrlBase}${this.requestUrl}/image/${imageId}/token`,
        this.serviceHelper.getHttpOptions(),
      ),
    );
  }

  /**
   * Starts downloading process for the given invoice document.
   * @param documentId
   */
  public async downloadInvoiceDocument(documentId: number): Promise<void> {
    const token = await this.getDownloadToken(documentId);
    const documentUrl = `${this.configService.apiUrlBase}${this.requestUrl}/image/${token}/${documentId}`;
    window.open(documentUrl, '_blank');
  }

  /**
   * Downloads and opens given invoice document in a new browser tab.
   * @param documentId
   */
  public async openInvoiceDocument(documentId: number): Promise<void> {
    const token = await this.getDownloadToken(documentId);
    const documentUrl = `${this.configService.apiUrlBase}${this.requestUrl}/image/${token}/${documentId}`;
    const documentBlob = await lastValueFrom(this.http.get(documentUrl, { responseType: 'blob' }));
    const documentLink = URL.createObjectURL(documentBlob);
    window.open(documentLink, '_blank');
  }

  public addInvoiceImage(invoiceImage: InvoiceImage): Observable<InvoiceUploadStatus> {
    const headers = new HttpHeaders().set('Content-Type', 'application/json');
    return this.http
      .post(this.configService.apiUrlBase + this.requestUrl, invoiceImage, {
        reportProgress: true,
        observe: 'events',
        responseType: 'text',
        headers,
      })
      .pipe(
        map((event): InvoiceUploadStatus => {
          if (event instanceof HttpResponse && event.body) {
            const id = Number(event.body);
            if (isNaN(id)) {
              throw new Error(`Unexpected response body: ${event.body}`);
            }
            return {
              status: 'completed',
              id,
            };
          }

          if (event.type === HttpEventType.UploadProgress) {
            const percentDone = Math.round((100 * event.loaded) / (event.total ?? 0));
            return {
              status: 'in-progress',
              progress: percentDone,
            };
          }

          return {
            status: 'in-progress',
            progress: 0,
          };
        }),
      );
  }
}
