import { Injectable } from '@angular/core'; 

@Injectable() 

export class ConfigService { 
    constructor() { 
    } 

    public get getAdalConfig(): any { 

        return { 
            tenant: 'TENANT_ID', 
            clientId: 'CLIENT_ID', 
            redirectUri: "window.location.origin + '/'", 
            postLogoutRedirectUri: window.location.origin + '/' 
        }; 
    } 
}