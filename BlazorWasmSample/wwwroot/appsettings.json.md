# appsettings.json

In order to be able to run this application locally, you need to create a `appsettings.json` file in the same folder with this documentation. This file is deliberately excluded from source control. The contents of that file must be:

``` JSON
{
  "app": {
    "clientId": "<client ID>",
    "tenantId": "<tenant name>.onmicrosoft.com"
  }
}
```

- `clientId`: The Client ID or Application ID of your application as registered in Azure AD.
- `tenantId`: The full tenant name or tenant ID (GUID) of the Azure AD tenant that your application is registered in.

> Note that you can structure your configuration file however you want, as long as you handle it properly in your startup configuration. In Blazor WebAssembly applications this is typically done in [`Program.cs`](../Program.cs).