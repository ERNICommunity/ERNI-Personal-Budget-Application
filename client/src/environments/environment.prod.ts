export const environment = {
  production: true,
  apiBaseUrl: 'https://ernipbaserver.azurewebsites.net/api/',
  protectedResourceMap: {
    'https://ernipbaserver.azurewebsites.net/**': ['api://536ddfb7-294a-4065-9be5-1c580d86fd42/pba_client']
  },
  msalLoginRedirectUri: 'https://ernipbaclient.azurewebsites.net',
  msalLogoutRedirectUri: 'https://ernipbaclient.azurewebsites.net',
  clientId: '106aed90-25b3-4d81-bbc0-9093d97668d9'
};
