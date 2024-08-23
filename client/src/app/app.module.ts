import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { RouterModule, TitleStrategy } from '@angular/router';
import { rootRouterConfig } from './app.routes';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import {
  MsalInterceptor,
  MSAL_INSTANCE,
  MSAL_GUARD_CONFIG,
  MSAL_INTERCEPTOR_CONFIG,
  MsalService,
  MsalBroadcastService,
} from '@azure/msal-angular';

import { MSALGuardConfigFactory, MSALInstanceFactory, MSALInterceptorConfigFactory } from './utils/msal';
import { SharedModule } from './shared/shared.module';
import { AuthenticatedComponent } from './authenticated/authenticated.component';
import { CustomTitleStrategy } from './services/custom-title-strategy.service';
import { BrowserUtils } from '@azure/msal-browser';

@NgModule({
  declarations: [AppComponent],
  bootstrap: [AppComponent],
  imports: [
    FormsModule,
    BrowserModule,
    SharedModule,
    RouterModule.forRoot(rootRouterConfig, {
      initialNavigation: !BrowserUtils.isInIframe() && !BrowserUtils.isInPopup() ? 'enabledNonBlocking' : 'disabled',
      bindToComponentInputs: true,
    }),
    ReactiveFormsModule,
    BrowserAnimationsModule,
    AuthenticatedComponent,
  ],
  providers: [
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
    {
      provide: TitleStrategy,
      useClass: CustomTitleStrategy,
    },
    MsalService,
    MsalBroadcastService,
    provideHttpClient(withInterceptorsFromDi()),
  ],
})
export class AppModule {}
