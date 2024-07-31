import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class BusyIndicatorService {
  public isBusy = true;
  public count = 0;

  public start() {
    this.count++;
    this.isBusy = this.count > 0;
  }

  public end() {
    this.count--;
    this.isBusy = this.count > 0;
  }
}
