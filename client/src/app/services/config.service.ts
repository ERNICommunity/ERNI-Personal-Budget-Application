import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({providedIn: 'root'})
export class ConfigService {
    constructor() {
    }

    public get apiUrlBase(): string {
        return environment.apiBaseUrl;
    }

    public get getOldestYear(): number {
        return 2015;
    }
}
