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
import { StepsModule } from "primeng/steps";
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { PanelModule } from 'primeng/panel';
import { TabMenuModule } from 'primeng/tabmenu';

@NgModule({
  declarations: [AlertComponent, FileUploadComponent],
  imports: [CommonModule, FormsModule, DividerModule, BrowserAnimationsModule,
  ],
  exports: [
    CommonModule,
    FormsModule,
    DividerModule,
    CalendarModule,
    InputTextModule,
    InputTextareaModule,
    InputNumberModule,
    DialogModule,
    StepsModule,
    ButtonModule,
    ProgressSpinnerModule,
    AlertComponent,
    FileUploadComponent,
    PanelModule,
    TabMenuModule,
  ],
})
export class SharedModule {}
