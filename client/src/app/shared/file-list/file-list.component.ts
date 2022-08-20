import { Component, Input, Output, EventEmitter } from '@angular/core';
import { InvoiceImageService } from '../../services/invoice-image.service';

export type InvoiceStatus =
    | { code: 'new' }
    | { code: 'in-progress'; progress?: number }
    | { code: 'saved'; id: number };

export type Invoice = {
    status: InvoiceStatus;
    name: string;
};

@Component({
    selector: 'app-file-list',
    templateUrl: './file-list.component.html',
    styleUrls: ['./file-list.component.css']
})
export class FileListComponent {
    @Input() uploadEnabled: boolean;

    @Input()
    public images: Invoice[];

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
            'fileDialog'
        ) as HTMLElement;
        element.click();
    }
}
