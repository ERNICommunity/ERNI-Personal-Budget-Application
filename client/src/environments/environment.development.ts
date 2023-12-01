export const environment = {
  production: false,
  apiBaseUrl: "http://localhost:5001/api/",
  protectedResourceMap: {
    "http://localhost:5001/**": [
      "api://536ddfb7-294a-4065-9be5-1c580d86fd42/pba_client",
    ],
  } as Record<string, Array<string>>,
  msalLoginRedirectUri: "http://localhost:4200",
  msalLogoutRedirectUri: "http://localhost:4200",
  clientId: "106aed90-25b3-4d81-bbc0-9093d97668d9",
};
