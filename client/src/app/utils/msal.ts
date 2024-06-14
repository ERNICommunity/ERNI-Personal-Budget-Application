import { MsalGuardConfiguration, MsalInterceptorConfiguration } from '@azure/msal-angular';
import {
  BrowserCacheLocation,
  InteractionType,
  IPublicClientApplication,
  LogLevel,
  PublicClientApplication,
} from '@azure/msal-browser';
import { environment } from '../../environments/environment';

export function MSALInstanceFactory(): IPublicClientApplication {
  return new PublicClientApplication({
    auth: {
      clientId: environment.clientId,
      redirectUri: environment.msalLoginRedirectUri,
      postLogoutRedirectUri: environment.msalLogoutRedirectUri,
      authority: 'https://login.microsoftonline.com/eb25818e-5bd5-49bf-99de-53e3e7b42630'
    },
    cache: {
      cacheLocation: BrowserCacheLocation.LocalStorage,
      storeAuthStateInCookie: false
    },
    system: {
      asyncPopups: true,
      allowNativeBroker: false,
      loggerOptions: {
        logLevel: LogLevel.Verbose,
        loggerCallback: (level: LogLevel, message: string) => {
          if (level === LogLevel.Error) {
            console.error(message);
          }
        },
        piiLoggingEnabled: false
      }
    }
  });
}

export function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
  const protectedResourceMap = new Map<string, Array<string>>();

  Object
    .keys(environment.protectedResourceMap)
    .forEach(key => protectedResourceMap.set(key, environment.protectedResourceMap[key]));

  return {
    interactionType: InteractionType.Popup,
    protectedResourceMap,
  };
}

export function MSALGuardConfigFactory(): MsalGuardConfiguration {
  return { interactionType: InteractionType.Popup };
}
