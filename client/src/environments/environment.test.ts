import { Environment } from "./environment.type";

export const environment: Environment = {
  production: true,
  apiBaseUrl: 'https://ernipbaserver-test.azurewebsites.net/api/',
  protectedResourceMap: {
    'https://ernipbaserver-test.azurewebsites.net/**': ['api://4ff8e6de-fdac-4249-b61b-1cc75a27ea19/pba_client']
  },
  msalLoginRedirectUri: 'https://ernipbaclient-test.azurewebsites.net',
  msalLogoutRedirectUri: 'https://ernipbaclient-test.azurewebsites.net',
  clientId: '6c1acfc8-935d-438d-b9be-208ca6493601'
};
