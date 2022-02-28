# Blazorade MSAL

Provides easy to use authentication and token acquisition for Blazor applications with the help of Microsoft Authentication Library. Supports both Blazor Server and Blazor WebAssembly applications.

## Getting Started

After you have installed the package to your application, refer to the [Getting Started](https://github.com/Blazorade/Blazorade-MSAL/wiki/Getting-Started) section on the package wiki for information on how to easily get started with Blazorade MSAL.

## Highlights

Blazorade MSAL facilitates authentication and authorization for instance with the following services.

- `BlazoradeMsalService` - A service class that handles all communication with the MSAL JavaScript library for you. You don't have to write a single line of JavaScript code in your application.
- `BlazoradeRequestFactory` - A factory service that creates [`HttpRequestMessage`](https://docs.microsoft.com/dotnet/api/system.net.http.httprequestmessage) instances. These request messages are configured with an access token provided by `BlazoradeMsalService` which enables you to easily call into APIs such as [Microsoft Graph](https://docs.microsoft.com/graph/api/overview).

These services are registered in your application's service collection with the `AddBlazoradeMsal` method as described in the [Getting Started](https://github.com/Blazorade/Blazorade-MSAL/wiki/Getting-Started#configure-blazorade-msal-for-your-application) section on the Blazoarde MSAL wiki.