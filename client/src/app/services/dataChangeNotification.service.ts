import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import { Request } from "../model/request/request";

export class Unit {  
}

@Injectable()
export class DataChangeNotificationService {
  private readonly _todos = new BehaviorSubject<Unit>(new Unit());

  readonly notifications$ = this._todos.asObservable();

  notify() {
    this._todos.next(new Unit());
  }
}