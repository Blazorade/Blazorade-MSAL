# Graph Client Sample Application

This sample application demonstrates how you use Blazorade MSAL to acquire an access token that you can use to access Microsoft Graph on behalf of the logged in user.

## Required Configuration

In order to be able to run this sample application on your local machine, you need to add your configuration information to it. Make sure that you have an application settings file available at `wwwroot/appsettings.json`. This file may contain sensitive information and is excluded from source control.

The contents of that file are described below.

``` JSON
{
    "app": {
        "clientId": <string>,
        "tenantId": <string>
    }
}
```

- `clientId`: The application ID (client ID) of the Azure AD registered application.
- `tenantId`: The tenant ID of the application.

## Azure AD Application Registration

Follow the steps below to register an application with Azure AD that you can use for this sample application.

1. Log in to the [Azure AD portal](https://aad.portal.azure.com/) and go to [App Registrations](https://aad.portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps).
2. Create a new application registration by clicking the *New registration* button.
3. Give the application a name.
4. Under *Redirect URI*, select Single-page application (SPA), and set the redirect URI to *https://localhost:7156/* (or whatever URI you have configured your sample to run on.)
5. Click *Register*.
6. Go to the *Authentication* blade for the application.
7. Under *Implicit grant and hybrid flows* make sure that both *Access tokens* and *ID tokens* checkboxes are checked.
8. Save the changes to the application registration.
