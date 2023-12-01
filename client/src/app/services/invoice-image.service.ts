import { Injectable } from "@angular/core";
import {
  HttpClient,
  HttpRequest,
  HttpEventType,
  HttpResponse,
} from "@angular/common/http";
import { ConfigService } from "./config.service";
import { Observable, Subject, firstValueFrom } from "rxjs";
import { ServiceHelper } from "./service.helper";
import { InvoiceImage } from "../model/InvoiceImage";

@Injectable({
  providedIn: "root",
})
export class InvoiceImageService {
  requestUrl = "InvoiceImage";

  constructor(
    private http: HttpClient,
    private configService: ConfigService,
    private serviceHelper: ServiceHelper
  ) {}

  public getInvoiceImages(
    requestId: number
  ): Observable<{ id: number; name: string }[]> {
    return this.http.get<{ id: number; name: string }[]>(
      this.configService.apiUrlBase + this.requestUrl + "/images/" + requestId,
      this.serviceHelper.getHttpOptions()
    );
  }

  public getDownloadToken(imageId: number) {
    return firstValueFrom(
      this.http.get<string>(
        `${this.configService.apiUrlBase}${this.requestUrl}/image/${imageId}/token`,
        this.serviceHelper.getHttpOptions()
      )
    );
  }

  public async getInvoiceImage(imageId: number) {
    const token = await this.getDownloadToken(imageId);
    const downloadLink =
      this.configService.apiUrlBase +
      this.requestUrl +
      "/image/" +
      token +
      "/" +
      imageId;

    window.open(downloadLink, "_blank");
  }

  public addInvoiceImage(invoiceImage: InvoiceImage): {
    progress: Observable<number>;
    id: Observable<number>;
  } {
    // // if(invoiceImage.file.size > 1048576)
    // // {
    // //   return throwError("Payload is too large - 413");
    // // }

    const request = new HttpRequest(
      "POST",
      this.configService.apiUrlBase + this.requestUrl,
      invoiceImage,
      {
        reportProgress: true,
      }
    );

    request.headers.set("Content-Type", "multipart/form-data");
    const progress = new Subject<number>();
    const id = new Subject<number>();
    this.http.request(request).subscribe(
      (event) => {
        if (event.type === HttpEventType.UploadProgress) {
          const percentDone = Math.round(
            (100 * event.loaded) / (event.total ?? 100)
          );
          progress.next(percentDone);
        } else if (event instanceof HttpResponse) {
          id.next(event.body as number);
          id.complete();
          progress.complete();
        }
      },
      (error) => {
        progress.error(error);
      }
    );
    return { progress, id };
  }
}
