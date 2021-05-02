import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse, HttpHeaders, HttpHeaderResponse } from '@angular/common/http';
import { ConfigService } from './config.service';
import { Observable, Subject, throwError } from 'rxjs';
import { ServiceHelper } from './service.helper';
import { InvoiceImage } from '../model/InvoiceImage';

@Injectable()

export class InvoiceImageService {
  requestUrl = "InvoiceImage";

  constructor(private http: HttpClient,
    private configService: ConfigService,
    private serviceHelper: ServiceHelper) { }

  public getInvoiceImages(requestId: number): Observable<[number,string][]> {
    return this.http.get<[number,string][]>(this.configService.apiUrlBase + this.requestUrl + "/images/" + requestId, this.serviceHelper.getHttpOptions());
  }

  public getDownloadToken(imageId: number): Promise<any> {
    return this.http.get<any>(`${this.configService.apiUrlBase}${this.requestUrl}/image/${imageId}/token`, this.serviceHelper.getHttpOptions()).toPromise();
  }

  public async getInvoiceImage(imageId : number)
  {

    var token = await this.getDownloadToken(imageId);
    var downloadLink = this.configService.apiUrlBase + this.requestUrl + "/image/" + token + "/" + imageId;

    window.open(downloadLink, "_blank");
  }

  public addInvoiceImage(invoiceImage: InvoiceImage): Observable<number> {

    // // if(invoiceImage.file.size > 1048576)
    // // {
    // //   return throwError("Payload is too large - 413");
    // // }

    let request = new HttpRequest('POST',
      this.configService.apiUrlBase + this.requestUrl,
      invoiceImage,
      {
        reportProgress: true
      });

    request.headers.set("Content-Type", "multipart/form-data")
    let progress = new Subject<number>();
    this.http.request(request).subscribe(event => {

      if (event.type === HttpEventType.UploadProgress) {
        let percentDone = Math.round(100 * event.loaded / event.total);
        progress.next(percentDone);
      }
      else if (event instanceof HttpResponse) {
        progress.complete();
      }
    });
    return progress;
  }
}
