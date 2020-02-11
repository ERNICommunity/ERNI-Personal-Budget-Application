import { Component, OnInit, Input } from '@angular/core';
import { InvoiceImageService } from '../services/invoice-image.service';
import { InvoiceImage } from '../model/InvoiceImage';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})


export class FileUploadComponent implements OnInit {
  @Input() requestId: number;
  constructor(private invoiceImageService : InvoiceImageService) { }

  ngOnInit() {  }

  public onImageAdded(files : FileList)
  {
    for(var i = 0; i< files.length; i++)
    {
      var image = new InvoiceImage();
      image.file = files[i];
      image.requestId = this.requestId;
      this.invoiceImageService.addInvoiceImage(image);
    }
  }
}
