import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { rootRouterConfig } from './app.routes';
import { ConfigService } from './services/config.service';
import { RequestsComponent } from './requests/requests.component';
import { RequestListComponent } from './requests/requestList/requestList.component';
import { RequestService } from './services/request.service';
import { HttpClientModule } from '@angular/common/http';
import { UsersComponent } from './users/users.component';
import { UserService } from './services/user.service';
import { NgbModule, NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { UserListComponent } from './users/userList/userList.component';
import { UserDetailComponent } from './users/userDetail/userDetail.component';
import { BudgetService } from './services/budget.service';
import { FormsModule } from '@angular/forms';
import { ServiceHelper } from './services/service.helper';
import { RequestAddComponent } from './requests/requestAdd/requestAdd.component';
import { RequestEditComponent } from './requests/requestEdit/requestEdit.component';
import { RequestDetailComponent } from './requests/requestDetail/requestDetail.component';
import { RequestMassComponent } from './requests/requestMass/requestMass.component';
import { OtherBudgetsComponent } from './budgets/otherBudgets/otherBudgets.component';
import { OtherBudgetsDetailComponent } from './budgets/otherBudgetsDetail/otherBudgetsDetail.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { BusyIndicatorService } from './services/busy-indicator.service';
import { ViewerGuard } from './services/guards/viewer.guard';
import { CreateUserComponent } from './users/create-user/create-user.component';
import { AlertComponent } from './directives/alert/alert.component';
import { NewRequestModalComponent } from './requests/requestAdd/newRequestModal.component';
import { RequestDetailModalComponent } from './requests/requestDetail/requestDetailModal.component';
import { EditRequestModalComponent } from './requests/requestEdit/editRequestModal.component';
import { DataChangeNotificationService } from './services/dataChangeNotification.service';
import { FileUploadComponent } from './file-upload/file-upload.component';
import { InvoiceImageService } from './services/invoice-image.service';
import { TeamBudgetService } from './services/team-budget.service';
import { ExportService } from './services/export.service';
import { WizardModule } from 'primeng-extensions-wizard/components/wizard.module';
import { StepsModule } from 'primeng/steps';
import { requestCreateReducer } from './requests/state/request.reducer'
import { requestApproveReducer } from './requests/state/request.reducer'
import { requestInvoiceReducer } from './requests/state/request.reducer'
import { requestRejectReducer } from './requests/state/request.reducer'

import {
    MsalInterceptor,
    MsalInterceptorConfiguration,
    MsalGuardConfiguration,
    MSAL_INSTANCE,
    MSAL_GUARD_CONFIG,
    MSAL_INTERCEPTOR_CONFIG,
    MsalService,
    MsalGuard,
    MsalBroadcastService,
  } from '@azure/msal-angular';

  import {
    BrowserCacheLocation,
    InteractionType,
    IPublicClientApplication,
    LogLevel,
    PublicClientApplication,
  } from '@azure/msal-browser';
  import { environment } from '../environments/environment';
  import { AdminRoleGuard } from './services/guards/admin-role.guard';
  import { AuthenticationService } from './services/authentication.service';
  import { StoreModule } from '@ngrx/store';

  export function MSALInstanceFactory(): IPublicClientApplication {
    return new PublicClientApplication({
      auth: {
        clientId: environment.clientId,
        redirectUri: environment.msalLoginRedirectUri,
        postLogoutRedirectUri: environment.msalLogoutRedirectUri,
        authority: 'https://login.microsoftonline.com/eb25818e-5bd5-49bf-99de-53e3e7b42630'
      },
      cache: {
        cacheLocation: BrowserCacheLocation.LocalStorage,
        storeAuthStateInCookie: false
      },
      system: {
        loggerOptions: {
          logLevel: LogLevel.Verbose,
          loggerCallback: (level: LogLevel, message: string, containsPii: boolean) => {
            if (level === LogLevel.Error) {
              console.error(message);
            }
          },
          piiLoggingEnabled: false
        }
      }
    });
  }

  export function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
    const protectedResourceMap = new Map<string, Array<string>>();

    Object
      .keys(environment.protectedResourceMap)
      .forEach(key => protectedResourceMap
      .set(key, environment.protectedResourceMap[key]));

    return {
      interactionType: InteractionType.Popup,
      protectedResourceMap,
    };
  }

  export function MSALGuardConfigFactory(): MsalGuardConfiguration {
    return { interactionType: InteractionType.Popup };
  }

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
        RequestAddComponent,
        RequestDetailComponent,
        EditRequestModalComponent,
        RequestEditComponent,
        RequestMassComponent,
        OtherBudgetsComponent,
        OtherBudgetsDetailComponent,
        AlertComponent,
        NewRequestModalComponent,
        RequestDetailModalComponent,
        FileUploadComponent
    ],
    imports: [
        NgbModule,
        FormsModule,
        HttpClientModule,
        BrowserModule,
        RouterModule.forRoot(rootRouterConfig),
        ReactiveFormsModule,
        StepsModule,
        StoreModule.forRoot({}, {}),
        StoreModule.forFeature('request', { requestCreateReducer }),
        StoreModule.forFeature('request', { requestApproveReducer }),
        StoreModule.forFeature('request', { requestRejectReducer }),
        StoreModule.forFeature('request', { requestInvoiceReducer })
    ],
    entryComponents: [
        NewRequestModalComponent, RequestDetailComponent, RequestEditComponent
    ],
    providers: [
        ConfigService,
        ServiceHelper,
        ExportService,
        RequestService,
        UserService,
        BudgetService,
        TeamBudgetService,
        BusyIndicatorService,
        DataChangeNotificationService,
        InvoiceImageService,
        ViewerGuard,
        AuthenticationService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: MsalInterceptor,
            multi: true,
          },
          {
            provide: MSAL_INSTANCE,
            useFactory: MSALInstanceFactory,
          },
          {
            provide: MSAL_GUARD_CONFIG,
            useFactory: MSALGuardConfigFactory,
          },
          {
            provide: MSAL_INTERCEPTOR_CONFIG,
            useFactory: MSALInterceptorConfigFactory,
          },
          MsalService,
          MsalGuard,
          MsalBroadcastService,
          AdminRoleGuard,
          WizardModule
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
