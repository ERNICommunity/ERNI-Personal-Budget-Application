import { Injectable } from '@angular/core'; 

@Injectable() 

export class ConfigService { 
    constructor() { 
    }

    public get apiUrlBase(): any {
        return 'https://ernipbaserver.azurewebsites.net/api/';
    }

    public get conditionsOfUseUrl() : any {
        return '';
    }

    public get getAdalConfig(): any { 

        return { 
            tenant: 'infomandli.onmicrosoft.com',
            clientId: 'a005097e-a58e-49c7-8c74-147aa4ace46c',
            redirectUri: window.location.origin + '/',
            postLogoutRedirectUri: window.location.origin + '/'
        }; 
    }
    
    public get getOldestYear(): number {
        return 2015;
    }
}