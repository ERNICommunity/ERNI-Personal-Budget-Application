import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RequestsComponent } from './requests.component';
import { ViewerGuard } from '../services/guards/viewer.guard';
import { RequestDetailComponent } from './requestDetail/requestDetail.component';
import { RequestEditComponent } from './requestEdit/requestEdit.component';
import { MsalGuard } from '@azure/msal-angular';
import { RequestListComponent } from './requestList/requestList.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { BasicRequestInfoEditorComponent } from './requestEdit/basic-request-info-editor/basic-request-info-editor.component';

@NgModule({
    declarations: [
        RequestsComponent,
        RequestListComponent,
        BasicRequestInfoEditorComponent
    ],
    exports: [BasicRequestInfoEditorComponent],
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        RouterModule.forChild([
            {
                path: 'requests',
                component: RequestsComponent,
                canActivate: [ViewerGuard],
                children: [
                    {
                        path: '',
                        redirectTo:
                            '1/pending/' + new Date().getFullYear().toString(),
                        pathMatch: 'full'
                    },
                    {
                        path: ':budgetType/:requestState/:year',
                        component: RequestListComponent,
                        canActivate: [ViewerGuard],
                        children: [
                            {
                                path: 'detail/:requestId',
                                component: RequestDetailComponent,
                                canActivate: [ViewerGuard]
                            },
                            {
                                path: 'edit/:id',
                                component: RequestEditComponent,
                                canActivate: [MsalGuard]
                            }
                        ]
                    }
                ]
            }
        ])
    ]
})
export class RequestsModule {}
