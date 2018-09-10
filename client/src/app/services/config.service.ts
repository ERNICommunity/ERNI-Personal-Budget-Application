import { Injectable } from '@angular/core'; 

@Injectable() 

export class ConfigService { 
    constructor() { 
    }

    public get apiUrlBase(): any {
        return 'http://localhost:64246/api/';
    }

    public get getAdalConfig(): any { 

        return { 
            tenant: 'TENANT_ID',
            clientId: 'CLIENT_ID',
            redirectUri: "window.location.origin + '/'",
            postLogoutRedirectUri: window.location.origin + '/'
        }; 
    }
    
    public get getOldestYear(): number {
        return 2015;
    }
}