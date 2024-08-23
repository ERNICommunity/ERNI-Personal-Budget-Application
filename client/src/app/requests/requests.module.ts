import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RequestsComponent } from './requests.component';
import { viewerGuard } from '../services/guards/viewer.guard';
import { RequestDetailComponent } from './requestDetail/requestDetail.component';
import { RequestEditComponent } from './requestEdit/requestEdit.component';
import { RequestListComponent } from './requestList/requestList.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { RequestMassComponent } from './requestMass/requestMass.component';
import { adminGuard } from '../services/guards/admin.guard';

@NgModule({
  declarations: [RequestsComponent, RequestListComponent, RequestDetailComponent, RequestMassComponent],
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    RouterModule.forChild([
      {
        path: '',
        component: RequestsComponent,
        canActivate: [viewerGuard],
        children: [
          {
            path: '',
            redirectTo: '1/pending/' + new Date().getFullYear().toString(),
            pathMatch: 'full',
          },
          {
            path: ':budgetType/:requestState/:year',
            component: RequestListComponent,
            children: [
              {
                path: 'detail/:requestId',
                component: RequestDetailComponent,
              },
              {
                path: 'edit/:id',
                component: RequestEditComponent,
              },
            ],
          },
          {
            path: 'mass-request',
            component: RequestMassComponent,
            canActivate: [adminGuard],
          },
        ],
      },
    ]),
  ],
})
export class RequestsModule {}
