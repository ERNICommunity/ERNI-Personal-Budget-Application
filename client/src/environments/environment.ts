// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  apiBaseUrl: 'http://localhost:5001/api/',
  protectedResourceMap: {
    'http://localhost:5001/**': ['api://4ff8e6de-fdac-4249-b61b-1cc75a27ea19/pba_client']
  },
  msalLoginRedirectUri: 'http://localhost:4200',
  msalLogoutRedirectUri: 'http://localhost:4200',
  clientId: '6c1acfc8-935d-438d-b9be-208ca6493601'
};