import { Component, input, inject, ChangeDetectionStrategy, output } from '@angular/core';
import { InvoiceImageService } from '../../services/invoice-image.service';
import { SharedModule } from '../shared.module';
import { FileSelectEvent } from 'primeng/fileupload';

export type InvoiceStatus = NewInvoiceStatus | InProgressInvoiceStatus | SavedInvoiceStatus;

export interface NewInvoiceStatus {
  code: 'new';
  file: File;
}
export interface InProgressInvoiceStatus {
  code: 'in-progress';
  progress: number;
}
export interface SavedInvoiceStatus {
  code: 'saved';
  id: number;
}

export interface Invoice {
  status: InvoiceStatus;
  name: string;
}

@Component({
  selector: 'app-file-list',
  templateUrl: './file-list.component.html',
  styleUrls: ['./file-list.component.css'],
  standalone: true,
  imports: [SharedModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FileListComponent {
  uploadEnabled = input<boolean>(true);
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
