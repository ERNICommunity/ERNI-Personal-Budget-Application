export const environment = {
  production: true,
  apiBaseUrl: 'https://ernipbaserver.azurewebsites.net/api/',
  protectedResourceMap: {
    'https://ernipbaserver.azurewebsites.net/**': ['api://536ddfb7-294a-4065-9be5-1c580d86fd42/pba_client']
  },
  msalLoginRedirectUri: 'https://ernipbaserver.azurewebsites.net',
  msalLogoutRedirectUri: 'https://ernipbaserver.azurewebsites.net'
};
