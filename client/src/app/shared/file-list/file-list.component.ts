import { Component, input, inject, ChangeDetectionStrategy, output, model, signal, effect } from '@angular/core';
import { InvoiceImageService } from '../../services/invoice-image.service';
import { SharedModule } from '../shared.module';
import { DragDropModule } from 'primeng/dragdrop';

export type Invoice = NotUploadedInvoice | UploadingInvoice | UploadedInvoice;

export interface NotUploadedInvoice {
  status: 'not-uploaded';
  file: File;
  name: string;
}
export interface UploadingInvoice {
  status: 'uploading';
  progress: number;
  name: string;
}
export interface UploadedInvoice {
  status: 'uploaded';
  id: number;
  name: string;
}

/** Maximum allowed file size - 2 MB */
const MAX_FILE_SIZE_IN_BYTES = 2_000_000;
const ACCEPTED_FILE_TYPES = ['image/png', 'image/jpeg', 'image/gif', 'application/pdf'];
const ALLOWED_FILE_EXTENSIONS = ['.png', '.jpg', '.jpeg', '.gif', '.pdf'];

interface DownloadingInvoice {
  status: 'downloading';
  name: string;
}

type UiInvoice = Invoice | DownloadingInvoice;

@Component({
  selector: 'app-file-list',
  templateUrl: './file-list.component.html',
  styleUrls: ['./file-list.component.css'],
  standalone: true,
  imports: [SharedModule, DragDropModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FileListComponent {
  uploadEnabled = input<boolean>(true);
  files = model.required<Invoice[]>();
  uiFileList = signal<UiInvoice[]>([]);

  newFileAdded = output<File[]>();

  errorMessage = signal<string | null>(null);

  readonly acceptedFileTypes = ACCEPTED_FILE_TYPES.join(',');

  #invoiceImageService = inject(InvoiceImageService);

  constructor() {
    effect(() => this.uiFileList.set([...this.files()]), { allowSignalWrites: true });
  }

  async download(file: UiInvoice): Promise<void> {
    if (file.status === 'uploaded') {
      this.updateFile({ ...file, status: 'downloading' });
      await this.#invoiceImageService.openInvoiceDocument(file.id);
      this.updateFile({ ...file, status: 'uploaded' });
    }
  }

  private updateFile(file: UiInvoice): void {
    this.uiFileList.update((files) =>
      files.map((f) => {
        if (f.name !== file.name) {
          return f;
        }
        return { ...file };
      }),
    );
  }

  onFileSelect(e: Event) {
    this.errorMessage.set(null);

    let fileList: FileList | null = null;
    // event triggered via button
    if (e.target instanceof HTMLInputElement && e.target.files) {
      fileList = e.target.files;
    }
    // event triggered via drag & drop
    if (e instanceof DragEvent && e.dataTransfer?.files) {
      fileList = e.dataTransfer.files;
    }
    if (!fileList) {
      return;
    }

    const files = Array.from(fileList);
    if (files.some((file) => file.size > MAX_FILE_SIZE_IN_BYTES)) {
      this.errorMessage.set('The file is too large. Allowed maximum size is 2MB.');
      return;
    }
    if (files.some((file) => !ACCEPTED_FILE_TYPES.includes(file.type))) {
      this.errorMessage.set(
        `Invalid file format. Only files with following extensions are allowed: ${ALLOWED_FILE_EXTENSIONS.join(', ')}`,
      );
      return;
    }

    this.newFileAdded.emit(files);
  }

  removeFile(file: UiInvoice): void {
    this.files.update((files) => files.filter((f) => f.name !== file.name));
  }
}
