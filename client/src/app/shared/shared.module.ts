import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AlertComponent } from "./alert/alert.component";
import { FileUploadComponent } from "./file-upload/file-upload.component";
import { DividerModule } from "primeng/divider";
import { CalendarModule } from "primeng/calendar";
import { FormsModule } from "@angular/forms";
import { InputTextareaModule } from "primeng/inputtextarea";
import { InputTextModule } from "primeng/inputtext";
import { InputNumberModule } from "primeng/inputnumber";
import { ProgressSpinnerModule } from "primeng/progressspinner";

@NgModule({
  declarations: [AlertComponent, FileUploadComponent],
  imports: [CommonModule, FormsModule, DividerModule],
  exports: [
    CommonModule,
    FormsModule,
    DividerModule,
    CalendarModule,
    InputTextModule,
    InputTextareaModule,
    InputNumberModule,
    ProgressSpinnerModule,
    AlertComponent,
    FileUploadComponent,
  ],
})
export class SharedModule {}
