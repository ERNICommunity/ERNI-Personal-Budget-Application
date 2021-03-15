import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable()

export class ConfigService {
    constructor() {
    }

    public get apiUrlBase(): any {
        return environment.apiBaseUrl;
    }

    public get getOldestYear(): number {
        return 2015;
    }
}