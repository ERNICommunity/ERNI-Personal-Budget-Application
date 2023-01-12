import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AlertComponent } from './alert/alert.component';
import { FileListComponent } from './file-list/file-list.component';
import { DividerModule } from 'primeng/divider';
import { CalendarModule } from 'primeng/calendar';
import { FormsModule } from '@angular/forms';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { StepsModule } from 'primeng/steps';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { PanelModule } from 'primeng/panel';
import { TabMenuModule } from 'primeng/tabmenu';
import { BrowserModule } from '@angular/platform-browser';
import { ToastModule } from 'primeng/toast';
import { MenuModule } from 'primeng/menu';
import { MessageService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { ToolbarModule } from 'primeng/toolbar';
import { ProgressBarModule } from 'primeng/progressbar';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { AccordionModule } from 'primeng/accordion';
import { DropdownModule } from 'primeng/dropdown';
import { BadgeModule } from 'primeng/badge';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { PickListModule } from 'primeng/picklist';
import { AuthDirective } from './directives/authDirective';

@NgModule({
    declarations: [AlertComponent, FileListComponent, AuthDirective],
    imports: [
        CommonModule,
        FormsModule,
        DividerModule,
        BrowserModule,
        BrowserAnimationsModule,
        ProgressBarModule,
        ButtonModule,
        ProgressSpinnerModule,
        MessageModule,
        MessagesModule
    ],
    exports: [
        CommonModule,
        FormsModule,
        DividerModule,
        CalendarModule,
        ConfirmDialogModule,
        InputTextModule,
        InputTextareaModule,
        InputNumberModule,
        DialogModule,
        ToastModule,
        ToolbarModule,
        TableModule,
        StepsModule,
        ButtonModule,
        ProgressSpinnerModule,
        ProgressBarModule,
        AlertComponent,
        FileListComponent,
        PanelModule,
        MenuModule,
        TabMenuModule,
        ProgressBarModule,
        ProgressSpinnerModule,
        AccordionModule,
        DropdownModule,
        BadgeModule,
        MessageModule,
        MessagesModule,
        PickListModule,
        AuthDirective
    ],
    providers: [MessageService]
})
export class SharedModule {}
