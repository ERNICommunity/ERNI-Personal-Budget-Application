import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { rootRouterConfig } from './app.routes';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RequestEditComponent } from './requests/requestEdit/requestEdit.component';
import { RequestDetailComponent } from './requests/requestDetail/requestDetail.component';
import { RequestMassComponent } from './requests/requestMass/requestMass.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { TeamBudgetModule } from './team-budget/team-budget.module';

import {
    MsalInterceptor,
    MSAL_INSTANCE,
    MSAL_GUARD_CONFIG,
    MSAL_INTERCEPTOR_CONFIG,
    MsalService,
    MsalGuard,
    MsalBroadcastService
} from '@azure/msal-angular';
import { MyBudgetModule } from './my-budget/my-budget.module';
import { RequestsModule } from './requests/requests.module';
import {
    MSALGuardConfigFactory,
    MSALInstanceFactory,
    MSALInterceptorConfigFactory
} from './utils/msal';
import { UsersModule } from './users/users.module';
import { BudgetsModule } from './budgets/budgets.module';
import { SharedModule } from './shared/shared.module';
import { StatisticsModule } from './statistics/statistics.module';

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        RequestDetailComponent,
        RequestEditComponent,
        RequestMassComponent
    ],
    imports: [
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
        TeamBudgetModule,
        StatisticsModule
    ],
    providers: [
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
        MsalGuard,
        MsalBroadcastService,
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
