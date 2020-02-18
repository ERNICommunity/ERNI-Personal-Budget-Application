import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';
import { ConfigService } from './config.service';
import { Observable, Subject } from 'rxjs';
import { InvoiceImage } from '../model/InvoiceImage';
import { ServiceHelper } from './service.helper';

@Injectable()

export class InvoiceImageService {
  requestUrl = "InvoiceImage";

  constructor(private http: HttpClient,
    private configService : ConfigService,
    private serviceHelper : ServiceHelper)
   {  }

   public getInvoiceImagesNames(requestId : number) : Observable<string[]>
   {
      return this.http.get<string[]>(this.configService.apiUrlBase + this.requestUrl + '/' +  requestId, this.serviceHelper.getHttpOptions());
   }

   public addInvoiceImage(invoiceImage : InvoiceImage) : Observable<number>
   {
     let formData: FormData = new FormData();
     formData.append('requestId', invoiceImage.requestId.toString());
     formData.append('fileKey',invoiceImage.file,invoiceImage.file.name);
     
     let request = new HttpRequest('POST',
     this.configService.apiUrlBase + this.requestUrl,
     formData,
     {
       reportProgress: true
     });

     request.headers.set("Content-Type","multipart/form-data")

     let progress = new Subject<number>();
    console.log("Sending");
     this.http.request(request).subscribe(event => {
       if(event.type === HttpEventType.UploadProgress)
       {
         let percentDone = Math.round(100 * event.loaded / event.total);
         progress.next(percentDone);
       }
       else if(event instanceof HttpResponse)
       {
         progress.complete();
       }

     })
     return progress;
   }
}
