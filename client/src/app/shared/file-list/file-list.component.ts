import { Component, Input, Output, EventEmitter } from "@angular/core";
import { InvoiceImageService } from "../../services/invoice-image.service";

export type InvoiceStatus =
  | { code: "new" }
  | { code: "in-progress"; progress?: number }
  | { code: "saved"; id: number };

export interface Invoice {
  status: InvoiceStatus;
  name: string;
}

@Component({
  selector: "app-file-list",
  templateUrl: "./file-list.component.html",
  styleUrls: ["./file-list.component.css"],
})
export class FileListComponent {
  @Input() uploadEnabled: boolean;

  @Input()
  public images: Invoice[];

  @Output()
  public newImageAdded = new EventEmitter<FileList>();

  constructor(private invoiceImageService: InvoiceImageService) {}

  download(imageId: number) {
    this.invoiceImageService.getInvoiceImage(imageId);
  }

  public onImageAdded(element: EventTarget) {
    this.newImageAdded.emit((element as HTMLInputElement).files);
  }

  public onButtonClick() {
    const element: HTMLElement = document.getElementById(
      "fileDialog"
    ) as HTMLElement;
    element.click();
  }
}
