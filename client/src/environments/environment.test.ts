export const environment = {
  production: true,
  apiBaseUrl: 'https://ernipbaserver-test.azurewebsites.net/api/',
  protectedResourceMap: {
    'https://ernipbaserver-test.azurewebsites.net/**': ['api://536ddfb7-294a-4065-9be5-1c580d86fd42/pba_client']
  },
  msalLoginRedirectUri: 'https://ernipbaserver-test.azurewebsites.net',
  msalLogoutRedirectUri: 'https://ernipbaserver-test.azurewebsites.net'
};
