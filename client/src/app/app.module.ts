import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";

import { AppComponent } from "./app.component";
import { RouterModule } from "@angular/router";
import { LoginComponent } from "./login/login.component";
import { rootRouterConfig } from "./app.routes";
import { ConfigService } from "./services/config.service";
import { RequestService } from "./services/request.service";
import { HttpClientModule } from "@angular/common/http";
import { UserService } from "./services/user.service";
import { NgbModule, NgbAlert } from "@ng-bootstrap/ng-bootstrap";
import { BudgetService } from "./services/budget.service";
import { FormsModule } from "@angular/forms";
import { ServiceHelper } from "./services/service.helper";
import { RequestAddComponent } from "./requests/requestAdd/requestAdd.component";
import { RequestEditComponent } from "./requests/requestEdit/requestEdit.component";
import { RequestDetailComponent } from "./requests/requestDetail/requestDetail.component";
import { RequestMassComponent } from "./requests/requestMass/requestMass.component";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { ReactiveFormsModule } from "@angular/forms";
import { BusyIndicatorService } from "./services/busy-indicator.service";
import { ViewerGuard } from "./services/guards/viewer.guard";
import { NewRequestModalComponent } from "./requests/requestAdd/newRequestModal.component";
import { DataChangeNotificationService } from "./services/dataChangeNotification.service";
import { InvoiceImageService } from "./services/invoice-image.service";
import { TeamBudgetService } from "./services/team-budget.service";
import { ExportService } from "./services/export.service";
import { WizardModule } from "primeng-extensions-wizard/components/wizard.module";
import { StepsModule } from "primeng/steps";
import { requestCreateReducer } from "./requests/state/request.reducer";
import { requestApproveReducer } from "./requests/state/request.reducer";
import { requestInvoiceReducer } from "./requests/state/request.reducer";
import { requestRejectReducer } from "./requests/state/request.reducer";

import {
  MsalInterceptor,
  MSAL_INSTANCE,
  MSAL_GUARD_CONFIG,
  MSAL_INTERCEPTOR_CONFIG,
  MsalService,
  MsalGuard,
  MsalBroadcastService,
} from "@azure/msal-angular";

import { AdminRoleGuard } from "./services/guards/admin-role.guard";
import { AuthenticationService } from "./services/authentication.service";
import { StoreModule } from "@ngrx/store";
import { MyBudgetModule } from "./my-budget/my-budget.module";
import { RequestsModule } from "./requests/requests.module";
import {
  MSALGuardConfigFactory,
  MSALInstanceFactory,
  MSALInterceptorConfigFactory,
} from "./utils/msal";
import { UsersModule } from "./users/users.module";
import { BudgetsModule } from "./budgets/budgets.module";
import { SharedModule } from "./shared/shared.module";

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RequestAddComponent,
    RequestDetailComponent,
    RequestEditComponent,
    RequestMassComponent,
    NewRequestModalComponent,
  ],
  imports: [
    NgbModule,
    FormsModule,
    HttpClientModule,
    BrowserModule,
    SharedModule,
    RouterModule.forRoot(rootRouterConfig),
    ReactiveFormsModule,
    MyBudgetModule,
    RequestsModule,
    UsersModule,
    BudgetsModule,
    StepsModule,
    StoreModule.forRoot({}, {}),
    StoreModule.forFeature("request", { requestCreateReducer }),
    StoreModule.forFeature("request", { requestApproveReducer }),
    StoreModule.forFeature("request", { requestRejectReducer }),
    StoreModule.forFeature("request", { requestInvoiceReducer }),
  ],
  entryComponents: [
    NewRequestModalComponent,
    RequestDetailComponent,
    RequestEditComponent,
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
    WizardModule,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
