import { ConfigService } from './config.service';
import { Injectable } from '@angular/core';

@Injectable()
export class BusyIndicatorService {
    public isBusy: boolean = true;
    public count: number = 0;

    public start() {
        this.count++;
        this.isBusy = this.count > 0;
    }

    public end() {
        this.count--;
        this.isBusy = this.count > 0;
    }
}
