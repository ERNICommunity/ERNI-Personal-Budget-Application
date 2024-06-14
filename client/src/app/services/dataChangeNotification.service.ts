import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export class Unit {
}

@Injectable({ providedIn: 'root' })
export class DataChangeNotificationService {
  private readonly _todos = new BehaviorSubject<Unit>(new Unit());

  readonly notifications$ = this._todos.asObservable();

  notify() {
    this._todos.next(new Unit());
  }
}
