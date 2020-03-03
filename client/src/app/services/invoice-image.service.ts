import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse, HttpHeaders } from '@angular/common/http';
import { ConfigService } from './config.service';
import { Observable, Subject } from 'rxjs';
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

  public getInvoiceImateUrl(imageId : number) : string 
  {
    return this.configService.apiUrlBase + this.requestUrl + "/image/" + imageId;
  }

  public addInvoiceImage(invoiceImage: InvoiceImage): Observable<number> {
    let formData: FormData = new FormData();
    formData.append('requestId', invoiceImage.requestId.toString());
    formData.append('file', invoiceImage.file, invoiceImage.file.name);

    let request = new HttpRequest('POST',
      this.configService.apiUrlBase + this.requestUrl,
      formData,
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
      else if (event.type === HttpEventType.ResponseHeader) {
        if (event.status === 409) {
          progress.error("Name already in DB");
        }
      }
      else if (event instanceof HttpResponse) {
        progress.complete();
      }
    });
    return progress;
  }
}
