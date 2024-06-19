export interface Alert {
  type: AlertType;
  message: string;
  alertId?: string;
  keepAfterRouteChange: boolean;
}

export enum AlertType {
  Success,
  Error,
  Info,
  Warning,
}
