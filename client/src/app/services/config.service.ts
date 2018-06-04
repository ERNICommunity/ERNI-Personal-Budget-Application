import { Injectable } from '@angular/core'; 

@Injectable() 

export class ConfigService { 
    constructor() { 
    } 

    public get getAdalConfig(): any { 

        return { 
            tenant: 'erni.ch',
            clientId: '7eb7f3c1-377e-433f-92fe-d657272d4b79',
            redirectUri: window.location.origin + '/',
            postLogoutRedirectUri: window.location.origin + '/' 
        }; 
    } 
}