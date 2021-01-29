# appsettings.local.json

In order to be able to run this sample application locally, you need to make sure that you have a `appsettings.local.json` file in the same folder with this documentation. That file is excluded from source control.

This file should have the following contents.

``` JSON
{
  "app": {
    "clientId": "<client ID>",
    "tenantId": "<tenant name>.onmicrosoft.com"
  }
}
```

The `clientId` attribute contains the application ID (client ID) of the application that you have registered in Azure AD. This is the identity of your application.

The `tenantId` attribute contains the name or ID of the tenant the application is registered in.
