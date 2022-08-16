import { Component, Input, Output, EventEmitter } from "@angular/core";
import { InvoiceImageService } from "../../services/invoice-image.service";

export class Image {
  id: number;
  name: string;
}

export class NewImage {
  name: string;
  file: File;
  progress?: number;
}

@Component({
  selector: "app-file-list",
  templateUrl: "./file-list.component.html",
  styleUrls: ["./file-list.component.css"],
})
export class FileListComponent {
  @Input() uploadEnabled: boolean;

  @Input()
  public images: (Image | NewImage)[];

  @Output()
  public onNewImageAdded = new EventEmitter<FileList>();

  constructor(private invoiceImageService: InvoiceImageService) {}

  download(imageId: number, imageName: string) {
    this.invoiceImageService.getInvoiceImage(imageId);
  }

  public onImageAdded(files: FileList) {
    this.onNewImageAdded.emit(files);
  }

  public onButtonClick() {
    let element: HTMLElement = document.getElementById(
      "fileDialog"
    ) as HTMLElement;
    element.click();
  }
}
