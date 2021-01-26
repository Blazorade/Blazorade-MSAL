using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace MsalTestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientId = args[0];
            var tenantId = args[1];
            var upn = args[2];

            var app = GetApp(clientId, tenantId);
            var account = await app.GetAccountAsync(upn);

            AuthenticationResult token = null;
            if(null != account)
            {

            }
            else
            {
                token = await app.AcquireTokenInteractive(new string[] { ".default" })
                    .WithLoginHint(upn)
                    .WithPrompt(Prompt.NoPrompt)
                    .ExecuteAsync();
            }

            Console.WriteLine("Hello World!");
        }


        static IPublicClientApplication GetApp(string clientId, string tenantId)
        {
            var app = PublicClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                //.WithDefaultRedirectUri()
                .Build();

            return app;
        }
    }
}
