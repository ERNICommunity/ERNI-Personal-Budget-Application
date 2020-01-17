import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import {AppComponent} from './app.component';
import {RouterModule} from '@angular/router';
import {LoginComponent} from './login/login.component';
import {rootRouterConfig} from './app.routes';
import {AdalService} from './services/adal.service';
import {ConfigService} from './services/config.service';
import {OAuthHandshakeModule} from './login-callback/oauth-callback.module';
import {AuthenticationGuard} from './services/guards/authentication.guard';
import {AdminGuard} from './services/guards/admin.guard'
import {RequestsComponent} from './requests/requests.component';
import {RequestListComponent} from './requests/requestList/requestList.component';
import {RequestService} from './services/request.service';
import {HttpClientModule} from '@angular/common/http';
import {UsersComponent} from './users/users.component';
import {UserService} from './services/user.service';
import {NgbModule, NgbAlert} from '@ng-bootstrap/ng-bootstrap';
import {UserListComponent} from './users/userList/userList.component';
import {UserDetailComponent} from './users/userDetail/userDetail.component';
import {MyBudgetComponent} from './budgets/myBudget/myBudget.component';
import {BudgetService} from './services/budget.service';
import {FormsModule} from '@angular/forms';
import { ServiceHelper } from './services/service.helper';
import { RequestAddComponent } from './requests/requestAdd/requestAdd.component';
import { RequestEditComponent } from './requests/requestEdit/requestEdit.component';
import { RequestDetailComponent } from './requests/requestDetail/requestDetail.component';
import { RequestMassComponent } from './requests/requestMass/requestMass.component';
import { OtherBudgetsComponent } from './budgets/otherBudgets/otherBudgets.component';
import { OtherBudgetsDetailComponent } from './budgets/otherBudgetsDetail/otherBudgetsDetail.component';
import { AuthInterceptor } from './interceptors/authInterceptor'
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { BusyIndicatorService } from './services/busy-indicator.service';
import { UnregisteredInterceptor } from './interceptors/unregisteredInterceptor';
import { ViewerGuard } from './services/guards/viewer.guard';
import { CreateUserComponent } from './users/create-user/create-user.component';
import { AlertComponent } from './directives/alert/alert.component';
import { BudgetComponent } from './budgets/budget/budget.component';
import { NewRequestModalComponent } from './requests/requestAdd/newRequestModal.component';
import { RequestDetailModalComponent } from './requests/requestDetail/requestDetailModal.component';
import { EditRequestModalComponent } from './requests/requestEdit/editRequestModal.component';

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        RequestsComponent,
        RequestListComponent,
        UsersComponent,
        UserListComponent,
        UserDetailComponent,
        CreateUserComponent,
        BudgetComponent,
        MyBudgetComponent,
        RequestAddComponent,
        RequestDetailComponent,
        EditRequestModalComponent,
        RequestEditComponent,
        RequestMassComponent,
        OtherBudgetsComponent,
        OtherBudgetsDetailComponent,
        AlertComponent,
        NewRequestModalComponent,
        RequestDetailModalComponent
    ],
    imports: [
        NgbModule,
        FormsModule,
        HttpClientModule,
        BrowserModule,
        RouterModule.forRoot(rootRouterConfig, { useHash: true }),
        OAuthHandshakeModule,
        ReactiveFormsModule
    ],
    entryComponents: [
        NewRequestModalComponent, RequestDetailComponent, RequestEditComponent
    ],
    providers: [
        AdalService,
        ConfigService,
        AuthenticationGuard,
        ServiceHelper,
        RequestService,
        UserService,
        BudgetService,
        BusyIndicatorService,
        AdminGuard,
        ViewerGuard,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: UnregisteredInterceptor,
            multi: true
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
