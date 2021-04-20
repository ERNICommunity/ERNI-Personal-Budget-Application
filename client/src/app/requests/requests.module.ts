import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RequestsComponent } from './requests.component';
import { ViewerGuard } from '../services/guards/viewer.guard';
import { RequestDetailModalComponent } from './requestDetail/requestDetailModal.component';
import { RequestEditComponent } from './requestEdit/requestEdit.component';
import { MsalGuard } from '@azure/msal-angular';
import { RequestFilter } from './requestFilter';
import { RequestListComponent } from './requestList/requestList.component';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';

const currentYear = "2021"; // = (new Date()).getFullYear();

@NgModule({
  declarations: [
    RequestsComponent,
    RequestListComponent,
    RequestDetailModalComponent,
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
            { path: '', redirectTo: 'pending/' + currentYear, pathMatch: 'full' },
            {
                path: 'pending/:year', component: RequestListComponent, data: { filter: RequestFilter.Pending }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [MsalGuard] },
                ]
            },
            {
                path: 'approved/:year', component: RequestListComponent, data: { filter: RequestFilter.Approved }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [MsalGuard] },
                ]
            },
            {
                path: 'approved-by-superior/:year', component: RequestListComponent, data: { filter: RequestFilter.ApprovedBySuperior }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [MsalGuard] },
                ]
            },
            {
                path: 'rejected/:year', component: RequestListComponent, data: { filter: RequestFilter.Rejected }, canActivate: [ViewerGuard],
                children: [
                    { path: 'detail/:requestId', component: RequestDetailModalComponent, canActivate: [ViewerGuard] },
                    { path: 'edit/:id', component: RequestEditComponent, canActivate: [MsalGuard] },
                ]
            }
        ]
    },
    ])
  ]
})
export class RequestsModule { }
