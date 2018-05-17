import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { rootRouterConfig } from './app.routes';
import { OAuthCallbackComponent } from './login-callback/oauth-callback.component';
import { AdalService } from './services/adal.service';
import { ConfigService } from './services/config.service';
import { OAuthCallbackHandler } from './login-callback/oauth-callback.guard';
import { OAuthHandshakeModule } from './login-callback/oauth-callback.module';
import { AuthenticationGuard } from './services/authenticated.guard';
import { RequestsComponent } from './requests/requests.component';
import { RequestService } from './services/request.service';
import { HttpClientModule } from '@angular/common/http';
import { UsersComponent } from './users/users.component';
import { UserService } from './services/user.service';
import {NgbModule, NgbAlert} from '@ng-bootstrap/ng-bootstrap';
import { UserListComponent } from './users/userList/userList.component';
import { UserDetailComponent } from './users/userDetail/userDetail.component';
import { BudgetsComponent } from './budgets/budgets.component';
import { BudgetService } from './services/budget.service';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    RequestsComponent,
    UsersComponent,
    UserListComponent,
    UserDetailComponent,
    BudgetsComponent
  ],
  imports: [
    NgbModule.forRoot(),
    FormsModule,
    HttpClientModule,
    BrowserModule,
    RouterModule.forRoot(rootRouterConfig, { useHash: true }),
    OAuthHandshakeModule
  ],
  providers: [
    AdalService, ConfigService, AuthenticationGuard, RequestService, UserService, BudgetService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
