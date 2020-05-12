import { Component, OnInit, Input, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Params} from '@angular/router';
import { Location } from '@angular/common';
import { Request } from '../../model/request/request';
import { RequestService } from '../../services/request.service';
import { FormGroup,  FormBuilder,  Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { InvoiceImageService } from '../../services/invoice-image.service';

@Component({
  selector: 'app-request-detail',
  templateUrl: 'requestDetail.component.html',
  styleUrls: ['requestDetail.component.css']
})
export class RequestDetailComponent implements OnInit {
  @ViewChild('downloadLink',{static : false}) downloadLink: ElementRef;
  request: Request;
  selectedDate : Date;
  requestForm: FormGroup;
  httpResponseError : string;
  images: [number, string][];
  
  constructor(private requestService: RequestService,
              private route: ActivatedRoute,
              private location: Location,
              public modal: NgbActiveModal,
              private invoiceImageService: InvoiceImageService){
               }

  ngOnInit() {
  }

  download(imageId: number, imageName: string) {
    this.invoiceImageService.getInvoiceImage(imageId).subscribe(blob => { this.processBlob(blob, imageName); })
  }

  processBlob(blob: Blob, name: string) {
    let fileObject = new File([blob], name);
    let url = window.URL.createObjectURL(fileObject);
    let link = this.downloadLink.nativeElement;
    link.setAttribute('download', name);
    link.href = url;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  public getRequest(id: number): void {
    this.requestService.getRequest(id)
      .subscribe(request => 
        { 
          this.request = request;
          this.selectedDate = new Date(request.date);
          this.invoiceImageService.getInvoiceImages(id).subscribe(names => {
            this.images = names;
          });
        },err => {
          this.httpResponseError = err.error
        });
    }
 
  goBack(): void {
    this.location.back();
  }
}
