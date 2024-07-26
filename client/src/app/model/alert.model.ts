export interface Alert {
  type: AlertType;
  message: string;
  alertId?: string;
  life?: number;
  keepAfterRouteChange: boolean;
}

export enum AlertType {
  Success,
  Error,
  Info,
  Warning,
}
