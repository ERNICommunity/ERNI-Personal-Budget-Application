import { Component, OnInit, Input, ViewChild, OnChanges, SimpleChanges, ElementRef } from '@angular/core';
import { InvoiceImageService } from '../../services/invoice-image.service';
import { InvoiceImage } from '../../model/InvoiceImage';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})


export class FileUploadComponent implements OnInit, OnChanges {
  @Input() requestIdInput: number;
  @Input() uploadEnabled: boolean;
  @ViewChild('file',{static : false}) file;
  @ViewChild('downloadLink',{static : false}) downloadLink: ElementRef;

  images: [number, string][];
  uploadingImages: uploadingImage[] = [];
  requestId: number;
  showError : boolean = false;

  constructor(private invoiceImageService: InvoiceImageService) { }

  ngOnInit() {  }

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
      this.showError = false;
    });
  }

  download(imageId: number, imageName: string) {
    this.invoiceImageService.getInvoiceImage(imageId);
  }

  public onImageAdded(files: FileList) {
    for (var i = 0; i < files.length; i++)
    {
      const fileReader = new FileReader();

      const selectedFile = files[i];

      fileReader.readAsDataURL(selectedFile);
      fileReader.onload = () => {
        if (fileReader.result) {

          const image: InvoiceImage = {
            id: this.requestId,
            data: fileReader.result.toString().replace('data:', '').replace(/^.+,/, ''),
            filename: selectedFile.name,
            mimeType: selectedFile.type
          }

          let newItem = new uploadingImage();

          newItem.name = image.filename;
          newItem.progress = 0;
          this.uploadingImages.push(newItem);

          this.invoiceImageService.addInvoiceImage(image).subscribe(progress => {
            newItem.progress = progress;
          }, (error) => {

            this.showError = true;
            this.uploadingImages.splice(this.uploadingImages.indexOf(newItem), 1);
          },
            () => {
              this.uploadingImages.splice(this.uploadingImages.indexOf(newItem), 1);
              this.updateImagesList();
            });
        }
      };
    }
  }
  public onButtonClick() {
    let element: HTMLElement = document.getElementById('fileDialog') as HTMLElement;
    element.click();
  }
}

class uploadingImage {
  name: string;
  progress: number;
}
