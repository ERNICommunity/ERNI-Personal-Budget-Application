export interface Environment {
  production: boolean;
  apiBaseUrl: string;
  protectedResourceMap: Record<string, string[]>;
  msalLoginRedirectUri: string;
  msalLogoutRedirectUri: string;
  clientId: string;
}
