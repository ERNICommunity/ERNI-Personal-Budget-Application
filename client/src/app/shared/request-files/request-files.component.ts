import { Component, ElementRef, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { Request } from '../../model/request/request';
import { RequestApprovalState } from '../../model/requestState';
import { InvoiceImageService } from '../../services/invoice-image.service';

@Component({
  selector: "app-request-files",
  templateUrl: "./request-files.component.html",
  styleUrls: ["./request-files.component.css"],
})
export class RequestFilesComponent implements OnInit, OnChanges {
  @Input() requestId: number;
  @Input() uploadEnabled: boolean;

  @ViewChild('downloadLink',{static : false}) downloadLink: ElementRef;

  images: [number, string][];
  public RequestState = RequestApprovalState; // this is required to be possible to use enum in view

  constructor(private invoiceImageService: InvoiceImageService) {}

  ngOnChanges(changes: SimpleChanges): void {
    this.invoiceImageService.getInvoiceImages(this.requestId).subscribe((names) => {
      this.images = names;
    });
  }

  public download(imageId: number, imageName: string): void {
    this.invoiceImageService.getInvoiceImage(imageId).subscribe(blob => { this.processBlob(blob, imageName); })
}

private processBlob(blob: Blob, name: string): void {
  let fileObject = new File([blob], name);
  let url = window.URL.createObjectURL(fileObject);
  let link = this.downloadLink.nativeElement;
  link.setAttribute('download', name);
  link.href = url;
  link.click();
  window.URL.revokeObjectURL(url);
}




  ngOnInit(): void {}
}
