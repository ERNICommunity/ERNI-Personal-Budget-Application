import { ChangeDetectionStrategy, Component, DestroyRef, inject, input, OnInit, signal } from '@angular/core';
import { AlertService } from '../../services/alert.service';
import { Alert, AlertType } from '../../model/alert.model';
import { Message } from 'primeng/api/message';
import { map } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AlertComponent implements OnInit {
  id = input<string>();

  #alertService = inject(AlertService);
  #destroyRef = inject(DestroyRef);
  alerts = signal<Message[]>([]);

  ngOnInit() {
    this.#alertService
      .onAlert(this.id())
      .pipe(
        map((alert) => {
          if (!alert.message) {
            // clear alerts when an empty alert is received
            this.alerts.set([]);
            return;
          }

          this.alerts.update((alerts) => [
            ...alerts,
            {
              severity: this.getSeverity(alert),
              summary: alert.message,
              life: alert.life,
            },
          ]);
        }),
        takeUntilDestroyed(this.#destroyRef),
      )
      .subscribe();
  }

  private getSeverity(alert: Alert) {
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
