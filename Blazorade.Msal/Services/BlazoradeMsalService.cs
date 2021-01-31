using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Blazorade.Core.Interop;
using Blazorade.Msal.Configuration;
using Blazorade.Msal.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorade.Msal.Services
{
    public class BlazoradeMsalService
    {
        public BlazoradeMsalService(BlazoradeMsalOptions options, IJSRuntime jsRuntime, NavigationManager navMan)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.JSRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
        }

        private BlazoradeMsalOptions Options;
        private IJSRuntime JSRuntime;
        private NavigationManager NavMan;



        public async Task<AuthenticationResult> AcquireTokenAsync(string loginHint = null, IEnumerable<string> scopes = null)
        {
            AuthenticationResult result = null;
            try
            {
                result = await this.AcquireTokenSilentAsync(loginHint, scopes);
            }
            // Deliberately just swallowing any error, since if we cannot get a token this way, then we use another fallback method.
            catch (FailureCallbackException) { }
            

            if (null == result)
            {
                try
                {
                    result = await this.AcquireTokenInteractiveAsync(loginHint, scopes);
                }
                catch (FailureCallbackException) { }
            }

            return result;
        }

        public async Task<AuthenticationResult> AcquireTokenInteractiveAsync(string loginHint = null, IEnumerable<string> scopes = null)
        {
            if(this.Options.InteractiveLoginMode == InteractiveLoginMode.Dialog)
            {
                return await this.AcquireTokenPopupAsync(loginHint, scopes);
            }
            else if(this.Options.InteractiveLoginMode == InteractiveLoginMode.Redirect)
            {
                await this.AcquireTokenRedirectAsync(loginHint, scopes);
            }

            return null;
        }

        public async Task<AuthenticationResult> AcquireTokenSilentAsync(string loginHint = null, IEnumerable<string> scopes = null, bool fallbackToDefaultAccount = false)
        {
            AuthenticationResult result = null;
            var module = await this.GetBlazoradeModuleAsync();

            if(this.Options.InteractiveLoginMode == InteractiveLoginMode.Redirect)
            {
                try
                {
                    result = await this.HandleRedirectPromiseAsync();
                }
                catch { }
            }

            if(null == result)
            {
                var data = this.CreateMsalData(loginHint: loginHint, scopes: scopes, fallbackToDefaultAccount: fallbackToDefaultAccount);
                var handler = new DotNetInstanceCallbackHandler<AuthenticationResult>(module, "acquireTokenSilent", data);
                result = await handler.GetResultAsync();
            }

            return result;
        }

        public async Task<AuthenticationResult> HandleRedirectPromiseAsync()
        {
            var module = await this.GetBlazoradeModuleAsync();
            var data = this.CreateMsalData(navigateToLoginRequestUrl: false);

            var handler = new DotNetInstanceCallbackHandler<AuthenticationResult>(module, "handleRedirectPromise", data);
            var result = await handler.GetResultAsync();
            return result;
        }

        public async Task LogoutAsync()
        {
            var module = await this.GetBlazoradeModuleAsync();
            var data = this.CreateMsalData();

            var handler = new DotNetInstanceCallbackHandler(module, "logout", data);
            await handler.GetResultAsync();
        }



        protected async Task<AuthenticationResult> AcquireTokenPopupAsync(string loginHint = null, IEnumerable<string> scopes = null)
        {
            var module = await this.GetBlazoradeModuleAsync();
            var data = this.CreateMsalData(loginHint: loginHint, scopes: scopes);

            var handler = new DotNetInstanceCallbackHandler<AuthenticationResult>(module, "acquireTokenPopup", data);
            return await handler.GetResultAsync();
        }

        protected async Task AcquireTokenRedirectAsync(string loginHint = null, IEnumerable<string> scopes = null)
        {
            var module = await this.GetBlazoradeModuleAsync();
            var data = this.CreateMsalData(loginHint: loginHint, scopes: scopes);

            var handler = new DotNetInstanceCallbackHandler(module, "acquireTokenRedirect", data);
            await handler.GetResultAsync();
        }



        private Dictionary<string, object> CreateMsalConfig(bool navigateToLoginRequestUrl = true)
        {
            var auth = new Dictionary<string, object>
            {
                { "clientId", this.Options.ClientId },
                { "authority", $"https://login.microsoftonline.com/{ this.Options.TenantId ?? "common" }" },
                { "navigateToLoginRequestUrl", navigateToLoginRequestUrl }
            };

            var redirectUri = this.CreateAbsoluteUri(this.Options.RedirectUrl);
            if (null != redirectUri)
            {
                auth["redirectUri"] = redirectUri;
            }

            var postLogoutRedirectUri = this.CreateAbsoluteUri(this.Options.PostLogoutUrl);
            if (null != postLogoutRedirectUri)
            {
                auth["postLogoutRedirectUri"] = postLogoutRedirectUri;
            }

            var msalConfig = new Dictionary<string, object>
            {
                { "auth", auth }
            };
            return msalConfig;
        }

        private Dictionary<string, object> CreateMsalData(string loginHint = null, IEnumerable<string> scopes = null, bool navigateToLoginRequestUrl = true, bool fallbackToDefaultAccount = false)
        {
            var msalConfig = this.CreateMsalConfig(navigateToLoginRequestUrl);

            var data = new Dictionary<string, object>
            {
                { "scopes", scopes?.Count() > 0 ? scopes : this.Options.DefaultScopes?.Count() > 0 ? this.Options.DefaultScopes : new string[] { ".default" } },
                { "msalConfig", msalConfig },
            };

            if(loginHint?.Length > 0)
            {
                data["loginHint"] = loginHint;
            }

            if (fallbackToDefaultAccount)
            {
                data["fallbackToDefaultAccount"] = true;
            }

            return data;
        }

        private Uri CreateAbsoluteUri(string relativeOrAbsoluteUrl)
        {
            if(relativeOrAbsoluteUrl?.Length > 0)
            {
                var uri = new Uri(relativeOrAbsoluteUrl, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri)
                {
                    uri = this.NavMan.ToAbsoluteUri(uri.ToString());
                }

                return uri;
            }
            return null;
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
