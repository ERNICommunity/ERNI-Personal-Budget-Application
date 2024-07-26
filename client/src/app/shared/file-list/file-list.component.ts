import {
  Component,
  input,
  inject,
  ChangeDetectionStrategy,
  output,
} from "@angular/core";
import { InvoiceImageService } from "../../services/invoice-image.service";
import { SharedModule } from "../shared.module";
import { FileSelectEvent } from "primeng/fileupload";

export type InvoiceStatus =
  | { code: "new"; file: File }
  | { code: "in-progress"; progress: number }
  | { code: "saved"; id: number };

export interface Invoice {
  status: InvoiceStatus;
  name: string;
}

@Component({
  selector: "app-file-list",
  templateUrl: "./file-list.component.html",
  styleUrls: ["./file-list.component.css"],
  standalone: true,
  imports: [SharedModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FileListComponent {
  uploadEnabled = input.required<boolean>();
  files = input.required<Invoice[]>();

  newImageAdded = output<File[]>();

  #invoiceImageService = inject(InvoiceImageService);

  download(imageId: number) {
    this.#invoiceImageService.getInvoiceImage(imageId);
  }

  onFileSelect(e: FileSelectEvent) {
    this.newImageAdded.emit(e.currentFiles);
  }
}
