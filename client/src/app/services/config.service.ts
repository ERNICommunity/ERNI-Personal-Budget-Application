import { Injectable } from '@angular/core'; 

@Injectable() 

export class ConfigService { 
    constructor() { 
    }

    public get apiUrlBase(): any {
        return 'http://localhost:64244/api/';
    }

    public get conditionsOfUseUrl() : any {
        return '';
    }

    public get getAdalConfig(): any { 

        return { 
            tenant: 'erni.ch',
            clientId: '6e1fa5b9-f4fb-42da-a09a-51c5bacb7622',
            redirectUri: window.location.origin + '/',
            postLogoutRedirectUri: window.location.origin + '/'
        }; 
    }
    
    public get getOldestYear(): number {
        return 2015;
    }
}