export const environment = {
    production: true,
    apiBaseUrl: 'https://ernipbaserver-prod.azurewebsites.net/api/',
    protectedResourceMap: {
        'https://ernipbaserver-prod.azurewebsites.net/**': [
            'api://536ddfb7-294a-4065-9be5-1c580d86fd42/pba_client'
        ]
    },
    msalLoginRedirectUri: 'https://pba.erninet.ch/msal-redirect.html',
    msalLogoutRedirectUri: 'https://pba.erninet.ch',
    clientId: '106aed90-25b3-4d81-bbc0-9093d97668d9'
};
