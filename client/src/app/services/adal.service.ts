import { ConfigService } from './config.service'; 
import { Injectable } from '@angular/core'; 

import * as AuthenticationContext from 'adal-angular'; 
import { UserService } from './user.service';

@Injectable() 
export class AdalService { 
private context: AuthenticationContext; 
constructor(private configService: ConfigService) { 
    this.context = new AuthenticationContext(configService.getAdalConfig); 
} 

login() { 
    localStorage.clear();
    this.context.login(); 
} 

logout() {
    localStorage.clear(); 
    this.context.logOut(); 
} 

handleCallback() { 
    this.context.handleWindowCallback(); 
}

public get userInfo() { 
    return this.context.getCachedUser(); 
} 

public get accessToken() { 
    return this.context.getCachedToken(this.configService.getAdalConfig.clientId); 
} 

public get isAuthenticated() { 
return this.userInfo && this.accessToken; 
    } 
} 

