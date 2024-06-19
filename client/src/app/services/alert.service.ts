import { Injectable } from "@angular/core";
import { Subject, Observable } from "rxjs";
import { Router, NavigationStart } from "@angular/router";
import { filter } from "rxjs/operators";
import { Alert, AlertType } from "../model/alert.model";

@Injectable({
  providedIn: "root",
})
export class AlertService {
  private subject: Subject<Alert> = new Subject<Alert>();
  private keepAfterRouteChange = false;

  constructor(private router: Router) {
    // clear alert messages on route change unless 'keepAfterRouteChange' flag is true
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        if (this.keepAfterRouteChange) {
          // only keep for a single route change
          this.keepAfterRouteChange = false;
        } else {
          // clear alert messages
          this.clear();
        }
      }
    });
  }

  // enable subscribing to alerts observable
  onAlert(alertId?: string): Observable<Alert> {
    return this.subject
      .asObservable()
      .pipe(filter((x) => x && x.alertId === alertId));
  }

  // convenience methods
  success(message: string, alertId?: string) {
    this.alert({
      message,
      type: AlertType.Success,
      alertId,
      keepAfterRouteChange: false,
    });
  }

  error(message: string, alertId?: string) {
    this.alert({
      message,
      type: AlertType.Error,
      alertId,
      keepAfterRouteChange: false,
    });
  }

  info(message: string, alertId?: string) {
    this.alert({
      message,
      type: AlertType.Info,
      alertId,
      keepAfterRouteChange: false,
    });
  }

  warn(message: string, alertId?: string) {
    this.alert({
      message,
      type: AlertType.Warning,
      alertId,
      keepAfterRouteChange: false,
    });
  }

  // main alert method
  alert(alert: Alert) {
    this.keepAfterRouteChange = alert.keepAfterRouteChange;
    this.subject.next(alert);
  }

  // clear alerts
  clear(alertId?: string) {
    this.subject.next({
      alertId: alertId,
      type: AlertType.Success,
      message: "",
      keepAfterRouteChange: false,
    });
  }
}
