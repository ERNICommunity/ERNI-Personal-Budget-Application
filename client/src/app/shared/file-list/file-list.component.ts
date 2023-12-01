import { Component, Input, Output, EventEmitter } from "@angular/core";
import { InvoiceImageService } from "../../services/invoice-image.service";
import { ButtonModule } from "primeng/button";
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { NgFor, NgIf } from "@angular/common";

export type InvoiceStatus =
  | { code: "new" }
  | { code: "in-progress"; progress?: number }
  | { code: "saved"; id: number };

export type Invoice = {
  status: InvoiceStatus;
  name: string;
};

@Component({
  selector: "app-file-list",
  templateUrl: "./file-list.component.html",
  styleUrls: ["./file-list.component.css"],
  standalone: true,
  imports: [NgFor, NgIf, ProgressSpinnerModule, ButtonModule],
})
export class FileListComponent {
  @Input({ required: true }) uploadEnabled!: boolean;

  @Input({ required: true })
  public images: Invoice[] = [];

  @Output()
  public newImageAdded = new EventEmitter<FileList>();

  constructor(private invoiceImageService: InvoiceImageService) {}

  download(imageId: number) {
    this.invoiceImageService.getInvoiceImage(imageId);
  }

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  public onImageAdded(event: any) {
    if (event.target.files) {
      this.newImageAdded.emit(event.target?.files);
    }
  }

  public onButtonClick() {
    const element: HTMLElement = document.getElementById(
      "fileDialog"
    ) as HTMLElement;
    element.click();
  }
}
