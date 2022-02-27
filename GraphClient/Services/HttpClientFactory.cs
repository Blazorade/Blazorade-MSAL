using Blazorade.Msal.Services;
using System.Net.Http.Headers;

namespace GraphClient.Services
{
    public class HttpClientFactory
    {
        public HttpClientFactory(BlazoradeMsalService msalService)
        {
            this.MsalService = msalService ?? throw new ArgumentNullException(nameof(msalService));
            this.Client = new HttpClient();
            this.Client.BaseAddress = new Uri("https://graph.microsoft.com/v1.0/", UriKind.Absolute);
        }

        private readonly BlazoradeMsalService MsalService;
        private readonly HttpClient Client;

        public async Task<HttpClient> GetHttpClientAsync(params string[] scopes)
        {
            var authResult = await this.MsalService.AcquireTokenAsync(scopes: scopes, fallbackToDefaultLoginHint: true);

            if (authResult?.AccessToken?.Length > 0)
            {
                this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            }

            return this.Client;
        }
    }
}
