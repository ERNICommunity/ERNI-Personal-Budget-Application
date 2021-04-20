import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AlertComponent } from './alert/alert.component';
import { FileUploadComponent } from './file-upload/file-upload.component';

@NgModule({
  declarations: [
    AlertComponent,
    FileUploadComponent,
  ],
  imports: [
    CommonModule
  ],
  exports: [
    AlertComponent,
    FileUploadComponent
  ]
})
export class SharedModule { }
