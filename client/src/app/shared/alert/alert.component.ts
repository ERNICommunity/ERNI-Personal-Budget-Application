import { Component, OnInit, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { AlertService } from '../../services/alert.service';
import { Alert, AlertType } from '../../model/alert.model';
import { Message } from 'primeng/api/message';

@Component({
    selector: 'app-alert',
    templateUrl: './alert.component.html',
    styleUrls: ['./alert.component.css']
})
export class AlertComponent implements OnInit {
    @Input() id: string;

    alerts: Message[] = [];
    subscription: Subscription;

    constructor(private alertService: AlertService) {}

    ngOnInit() {
        this.subscription = this.alertService
            .onAlert(this.id)
            .subscribe((alert) => {
                if (!alert.message) {
                    // clear alerts when an empty alert is received
                    this.alerts = [];
                    return;
                }
                this.alerts = [
                    ...this.alerts,
                    {
                        severity: this.cssClass(alert),
                        summary: alert.message
                        //detail: alert.message
                    }
                ];
            });
    }

    ngOnDestroy() {
        // unsubscribe to avoid memory leaks
        this.subscription.unsubscribe();
    }

    hide() {
        this.alerts = [];
    }

    cssClass(alert: Alert) {
        if (!alert) {
            return;
        }

        // return css class based on alert type
        switch (alert.type) {
            case AlertType.Success:
                return 'success';
            case AlertType.Error:
                return 'error';
            case AlertType.Info:
                return 'info';
            case AlertType.Warning:
                return 'warning';
        }
    }
}
