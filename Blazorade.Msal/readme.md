# Blazorade MSAL

Provides easy to use authentication and token acquisition for Blazor applications with the help of Microsoft Authentication Library. Supports both Blazor Server and Blazor WebAssembly applications.

## Getting Started

After you have installed the package to your application, refer to the [Getting Started](https://github.com/Blazorade/Blazorade-MSAL/wiki/Getting-Started) section on the package wiki for information on how to easily get started with Blazorade MSAL.

## Highlights

Blazorade MSAL facilitates authentication and authorization for instance with the following services.

- `BlazoradeMsalService` - A service class that handles all communication with the MSAL JavaScript library for you. You don't have to write a single line of JavaScript code in your application.
- `BlazoradeRequestFactory` - A factory service that creates [`HttpRequestMessage`](https://docs.microsoft.com/dotnet/api/system.net.http.httprequestmessage) instances. These request messages are configured with an access token provided by `BlazoradeMsalService` which enables you to easily call into APIs such as [Microsoft Graph](https://docs.microsoft.com/graph/api/overview).

These services are registered in your application's service collection with the `AddBlazoradeMsal` method as described in the [Getting Started](https://github.com/Blazorade/Blazorade-MSAL/wiki/Getting-Started#configure-blazorade-msal-for-your-application) section on the Blazoarde MSAL wiki.

## Sample Applications

The Github repository for Blazorade MSAL contains several sample applications that demonstrate how you can leverage Blazorade MSAL in your own application.

- [**GraphClient**](https://github.com/Blazorade/Blazorade-MSAL/tree/main/GraphClient) - A WebAssembly application that demonstrates how to send HTTP requests to Microsoft Graph with the help of Blazorade MSAL. Can be applied to any other REST API that requires access tokens.
- [**BlazorServerSample**](https://github.com/Blazorade/Blazorade-MSAL/tree/main/BlazorServerSample) - A Blazor Server application that demonstrates how you can make use of the on-demand token acquisition provided by Blazorade MSAL.
- [**BlazorWasmSample**](https://github.com/Blazorade/Blazorade-MSAL/tree/main/BlazorWasmSample) - The same as BlazorServerSample but implemented as a Blazor WebAssembly application. Shares much of the features with the Server sample through the [SharedComponentsSample](https://github.com/Blazorade/Blazorade-MSAL/tree/main/SharedComponentsSample) component library.

## Version Highlights

This section lists the main improvements in each published version.

### v2.1.0

This version includes the following pull requests.

- [#24 Changed methods on `BlazoradeMsalService` to virtual](https://github.com/Blazorade/Blazorade-MSAL/pull/24)
- [#25 Added HasTokenAsync method](https://github.com/Blazorade/Blazorade-MSAL/pull/25)

## Further Reading

To learn more, read these [Blazorade MSAL articles](https://mikaberglund.com/tag/blazorade-msal/) on Mika Berglund's blog.