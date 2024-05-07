import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { rootRouterConfig } from './app.routes';
import { ConfigService } from './services/config.service';
import { RequestService } from './services/request.service';
import { HttpClientModule } from '@angular/common/http';
import { UserService } from './services/user.service';
import { BudgetService } from './services/budget.service';
import { FormsModule } from '@angular/forms';
import { ServiceHelper } from './services/service.helper';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { BusyIndicatorService } from './services/busy-indicator.service';
import { ViewerGuard } from './services/guards/viewer.guard';
import { DataChangeNotificationService } from './services/dataChangeNotification.service';
import { InvoiceImageService } from './services/invoice-image.service';
import { TeamBudgetService } from './services/team-budget.service';
import { ExportService } from './services/export.service';

import {
    MsalInterceptor,
    MSAL_INSTANCE,
    MSAL_GUARD_CONFIG,
    MSAL_INTERCEPTOR_CONFIG,
    MsalService,
    MsalBroadcastService
} from '@azure/msal-angular';

import { AdminRoleGuard } from './services/guards/admin-role.guard';
import { AuthenticationService } from './services/authentication.service';
import {
    MSALGuardConfigFactory,
    MSALInstanceFactory,
    MSALInterceptorConfigFactory
} from './utils/msal';
import { SharedModule } from './shared/shared.module';
import { StatisticsService } from './services/statistics.service';
import { AuthenticatedComponent } from './authenticated/authenticated.component';
import { AutheticatedGuard } from './services/guards/authenticated.guard';
import { SuperiorGuard } from './services/guards/superior.guard';

@NgModule({
    declarations: [AppComponent, LoginComponent, AuthenticatedComponent],
    imports: [
        FormsModule,
        HttpClientModule,
        BrowserModule,
        SharedModule,
        RouterModule.forRoot(rootRouterConfig),
        ReactiveFormsModule,
        BrowserAnimationsModule
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
        StatisticsService,
        ViewerGuard,
        AutheticatedGuard,
        SuperiorGuard,
        AuthenticationService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: MsalInterceptor,
            multi: true
        },
        {
            provide: MSAL_INSTANCE,
            useFactory: MSALInstanceFactory
        },
        {
            provide: MSAL_GUARD_CONFIG,
            useFactory: MSALGuardConfigFactory
        },
        {
            provide: MSAL_INTERCEPTOR_CONFIG,
            useFactory: MSALInterceptorConfigFactory
        },
        MsalService,
        MsalBroadcastService,
        AdminRoleGuard
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
