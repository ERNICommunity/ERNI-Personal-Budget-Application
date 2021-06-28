import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RequestsComponent } from './requests.component';
import { ViewerGuard } from '../services/guards/viewer.guard';
import { RequestDetailModalComponent } from './requestDetail/requestDetailModal.component';
import { RequestEditComponent } from './requestEdit/requestEdit.component';
import { MsalGuard } from '@azure/msal-angular';
import { RequestListComponent } from './requestList/requestList.component';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { BasicRequestInfoEditorComponent } from './requestEdit/basic-request-info-editor/basic-request-info-editor.component';
import { RequestApprovalState } from '../model/requestState';

@NgModule({
  declarations: [
    RequestsComponent,
    RequestListComponent,
    RequestDetailModalComponent,
    BasicRequestInfoEditorComponent
  ],
  exports: [
    BasicRequestInfoEditorComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    NgbModule,
    SharedModule,
    RouterModule.forChild([
      {
        path: 'requests', component: RequestsComponent, canActivate: [ViewerGuard],
        children: [
            { path: '', redirectTo: 'personal/pending/' + (new Date()).getFullYear().toString(), pathMatch: 'full' },
            {
                path: ':budgetType/:requestState/:year', component: RequestListComponent, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [MsalGuard] },
                ]
            },
        ]
    },
    ])
  ]
})
export class RequestsModule { }
