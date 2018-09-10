import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';

import {AppComponent} from './app.component';
import {HomeComponent} from './home/home.component';
import {RouterModule} from '@angular/router';
import {LoginComponent} from './login/login.component';
import {rootRouterConfig} from './app.routes';
import {OAuthCallbackComponent} from './login-callback/oauth-callback.component';
import {AdalService} from './services/adal.service';
import {ConfigService} from './services/config.service';
import {OAuthCallbackHandler} from './login-callback/oauth-callback.guard';
import {OAuthHandshakeModule} from './login-callback/oauth-callback.module';
import {AuthenticationGuard} from './services/authenticated.guard';
import {RequestsComponent} from './requests/requests.component';
import {RequestListComponent} from './requests/requestList/requestList.component';
import {RequestService} from './services/request.service';
import {HttpClientModule} from '@angular/common/http';
import {UsersComponent} from './users/users.component';
import {CategoriesComponent} from './categories/categories.component';
import {UserService} from './services/user.service';
import {NgbModule, NgbAlert} from '@ng-bootstrap/ng-bootstrap';
import {UserListComponent} from './users/userList/userList.component';
import {UserDetailComponent} from './users/userDetail/userDetail.component';
import {MyBudgetComponent} from './budgets/myBudget/myBudget.component';
import {BudgetService} from './services/budget.service';
import {FormsModule} from '@angular/forms';
import {CategoryListComponent} from "./categories/categoryList/categoryList.component";
import {CategoryDetailComponent} from "./categories/categoryDetail/categoryDetail.component";
import {CategoryService} from "./services/category.service";
import { ServiceHelper } from './services/service.helper';
import { RequestAddComponent } from './requests/requestAdd/requestAdd.component';
import { RequestDetailComponent } from './requests/requestDetail/requestDetail.component';
import { OtherBudgetsComponent } from './budgets/otherBudgets/otherBudgets.component';
import { OtherBudgetsDetailComponent } from './budgets/otherBudgetsDetail/otherBudgetsDetail.component';
import { AuthInterceptor } from './interceptors/authInterceptor'
import { HTTP_INTERCEPTORS } from '@angular/common/http';


@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        LoginComponent,
        RequestsComponent,
        RequestListComponent,
        UsersComponent,
        CategoriesComponent,
        UserListComponent,
        UserDetailComponent,
        MyBudgetComponent,
        CategoriesComponent,
        CategoryListComponent,
        CategoryDetailComponent,
        RequestAddComponent,
        RequestDetailComponent,
        OtherBudgetsComponent,
        OtherBudgetsDetailComponent
    ],
    imports: [
        NgbModule.forRoot(),
        FormsModule,
        HttpClientModule,
        BrowserModule,
        RouterModule.forRoot(rootRouterConfig, {useHash: true}),
        OAuthHandshakeModule
    ],
    providers: [
        AdalService, ConfigService, AuthenticationGuard, ServiceHelper, RequestService, UserService, BudgetService, CategoryService, 
        {
             provide: HTTP_INTERCEPTORS,
             useClass: AuthInterceptor,
             multi: true 
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
