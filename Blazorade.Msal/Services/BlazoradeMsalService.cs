using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Blazorade.Core.Interop;
using Blazorade.Msal.Configuration;
using Blazorade.Msal.Security;
using Microsoft.JSInterop;

namespace Blazorade.Msal.Services
{
    public class BlazoradeMsalService
    {
        public BlazoradeMsalService(BlazoradeMsalOptions options, IJSRuntime jsRuntime)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.JSRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        private BlazoradeMsalOptions Options;
        private IJSRuntime JSRuntime;



        public async Task<AuthenticationResult> AcquireTokenAsync(string loginHint)
        {
            AuthenticationResult result = null;
            try
            {
                result = await this.AcquireTokenSilentAsync(loginHint);
            }
            // Deliberately just swallowing any error, since if we cannot get a token this way, then we use another fallback method.
            catch (FailureCallbackException) { }
            

            if (null == result)
            {
                try
                {
                    result = await this.AcquireTokenInteractiveAsync(loginHint);
                }
                catch (FailureCallbackException) { }
            }

            return result;
        }

        public async Task<AuthenticationResult> AcquireTokenInteractiveAsync(string loginHint)
        {
            if(this.Options.InteractiveLoginMode == InteractiveLoginMode.Dialog)
            {
                return await this.AcquireTokenPopupAsync(loginHint);
            }
            else if(this.Options.InteractiveLoginMode == InteractiveLoginMode.Redirect)
            {
                return await this.AcquireTokenRedirectAsync(loginHint);
            }

            return null;
        }

        public async Task<AuthenticationResult> AcquireTokenSilentAsync(string loginHint)
        {
            var module = await this.GetBlazoradeModuleAsync();
            var data = this.CreateMsalData(loginHint);

            var handler = new DotNetInstanceCallbackHandler<AuthenticationResult>(module, "acquireTokenSilent", data);
            return await handler.GetResultAsync();
        }



        protected async Task<AuthenticationResult> AcquireTokenPopupAsync(string loginHint)
        {
            var module = await this.GetBlazoradeModuleAsync();
            var data = this.CreateMsalData(loginHint);

            var handler = new DotNetInstanceCallbackHandler<AuthenticationResult>(module, "acquireTokenPopup", data);
            return await handler.GetResultAsync();
        }

        protected async Task<AuthenticationResult> AcquireTokenRedirectAsync(string loginHint)
        {

            return null;
        }



        private Dictionary<string, object> CreateMsalData(string loginHint)
        {
            var data = new Dictionary<string, object>
            {
                { "loginHint", loginHint },
                {
                    "msalConfig",
                    new Dictionary<string, object>
                    {
                        {
                            "auth",
                            new Dictionary<string, string>
                            {
                                { "clientId", this.Options.ClientId },
                                { "authority", $"https://login.microsoftonline.com/{ this.Options.TenantId ?? "common" }" }
                            }
                        }
                    }
                },
            };

            return data;
        }

        private IJSObjectReference _BlazoradeModule;
        private async Task<IJSObjectReference> GetBlazoradeModuleAsync()
        {
            var msalModule = await this.GetMsalModuleAsync();
            return _BlazoradeModule ??= await this.JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Blazorade.Msal/js/blazoradeMsal.js").AsTask();
        }

        private IJSObjectReference _MsalModule;
        private async Task<IJSObjectReference> GetMsalModuleAsync()
        {
            return _MsalModule ??= await this.JSRuntime.InvokeAsync<IJSObjectReference>("import", $"https://alcdn.msftauth.net/browser/{this.Options.MsalVersion}/js/msal-browser.min.js").AsTask();
        }
    }
}
