import { Component, OnInit, Input, ViewChild, OnChanges, SimpleChange, SimpleChanges } from '@angular/core';
import { InvoiceImageService } from '../services/invoice-image.service';
import { InvoiceImage } from '../model/InvoiceImage';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})


export class FileUploadComponent implements OnInit, OnChanges {
  @Input() requestIdInput: number;
  @ViewChild('file') file;

  images: string[];
  uploadingImages: uploadingImage[] = [];
  requestId: number;

  constructor(private invoiceImageService: InvoiceImageService) { }

  ngOnInit() { }

  ngOnChanges(changes: SimpleChanges) {

    if (!changes.requestIdInput || changes.requestIdInput.currentValue == undefined) {
      return;
    }

    this.requestId = changes.requestIdInput.currentValue;
    this.updateImagesList();
  }

  updateImagesList() {
    this.invoiceImageService.getInvoiceImagesNames(this.requestId).subscribe(names => {
      this.images = names;
      console.log(names);
    });
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
      console.log('new item pushed');

      this.invoiceImageService.addInvoiceImage(image).subscribe(progress => {
        newItem.progress = progress;
        console.log(progress);
      }, (error) => {
        console.log("Error ocurred");
        console.log(error);
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

//Is this valid ???

class uploadingImage {
  name: string;
  progress: number;
}
