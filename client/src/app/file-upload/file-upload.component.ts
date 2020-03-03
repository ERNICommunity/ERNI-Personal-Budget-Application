import { Component, OnInit, Input, ViewChild, OnChanges, SimpleChanges, ElementRef } from '@angular/core';
import { InvoiceImageService } from '../services/invoice-image.service';
import { InvoiceImage } from '../model/InvoiceImage';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})


export class FileUploadComponent implements OnInit, OnChanges {
  @Input() requestIdInput: number;
  @ViewChild('file') file;
  @ViewChild('downloadLink') downloadLink : ElementRef;

  images: [number,string][];
  uploadingImages: uploadingImage[] = [];
  requestId: number;

  constructor(private invoiceImageService: InvoiceImageService) { }

  ngOnInit() { 
  }

  ngOnChanges(changes: SimpleChanges) {

    if (!changes.requestIdInput || changes.requestIdInput.currentValue == undefined) {
      return;
    }

    this.requestId = changes.requestIdInput.currentValue;
    this.updateImagesList();
  }

  updateImagesList() {
    this.invoiceImageService.getInvoiceImages(this.requestId).subscribe(names => {
      this.images = names;
    });
  }

  download(imageId : number )
  {
    let url = this.invoiceImageService.getInvoiceImateUrl(imageId);
    let link = this.downloadLink.nativeElement;
    link.href = url;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  public onImageAdded(files: FileList) {
    for (var i = 0; i < files.length; i++) {
      var image = new InvoiceImage();
      image.file = files[i];
      image.requestId = this.requestId;

      let newItem = new uploadingImage();

      newItem.name = image.file.name;
      newItem.progress = 0;
      this.uploadingImages.push(newItem);

      this.invoiceImageService.addInvoiceImage(image).subscribe(progress => {
        newItem.progress = progress;
      }, (error) => {
        this.uploadingImages.splice(this.uploadingImages.indexOf(newItem), 1);
      },
        () => {
          this.uploadingImages.splice(this.uploadingImages.indexOf(newItem), 1);
          this.updateImagesList();
        });
    }
  }
  public onButtonClick(event: any) {
    let element: HTMLElement = document.getElementById('fileDialog') as HTMLElement;
    element.click();
  }
}

class uploadingImage {
  name: string;
  progress: number;
}