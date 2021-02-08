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
using Microsoft.Extensions.DependencyInjection;

namespace Blazorade.Msal.Services
{
    /// <summary>
    /// A service class implementation for working with tokens acquired by Microsoft Authentication Library.
    /// </summary>
    /// <remarks>
    /// An instance of this class is added to the services collection using one of the 
    /// <see cref="ServiceCollectionExtensionMethods.AddBlazoradeMsal"/> methods in your application's
    /// startup class. It can then be injected into your application.
    /// </remarks>
    public class BlazoradeMsalService
    {
        /// <summary>
        /// Creates an instance of the service class.
        /// </summary>
        public BlazoradeMsalService(BlazoradeMsalOptions options, IJSRuntime jsRuntime, NavigationManager navMan)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.JSRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
        }

        private BlazoradeMsalOptions Options;
        private IJSRuntime JSRuntime;
        private NavigationManager NavMan;



        /// <summary>
        /// Acquires a token with the given parameters.
        /// </summary>
        /// <remarks>
        /// This method first tries to acquire the token silently using the <see cref="AcquireTokenSilentAsync"/> method.
        /// If that failes, then the interactive option is used using the <see cref="AcquireTokenInteractiveAsync"/> method.
        /// </remarks>
        /// <param name="loginHint">
        /// The username to acquire the token for. If the token cannot be acquired silently, this value will also be
        /// passed to the interactive method to minimize the amount of information the user has to enter.
        /// </param>
        /// <param name="scopes">
        /// The scopes that must be included in the acquired token. If the user has not consented to one or more of these tokens, 
        /// the user will be taken to the interactive mode. If not specified, the default configured scopes will be used.
        /// </param>
        /// <param name="fallbackToDefaultLoginHint">
        /// Specifies whether to fall back to the default login hint. The default login hint is the login hint that was previously used to
        /// acquire a token.
        /// </param>
        /// <param name="prompt">
        /// Defines a prompt behaviour, in case this method falls back to interactive login.
        /// </param>
        public async Task<AuthenticationResult> AcquireTokenAsync(string loginHint = null, IEnumerable<string> scopes = null, bool fallbackToDefaultLoginHint = false, LoginPrompt? prompt = null)
        {
            AuthenticationResult result = null;

            // If prompt is specified, and it is something else than None, then we must skip the silent acquisition.

            if(!prompt.HasValue || prompt == LoginPrompt.None)
            {
                try
                {
                    result = await this.AcquireTokenSilentAsync(loginHint: loginHint, scopes: scopes, fallbackToDefaultLoginHint: fallbackToDefaultLoginHint);
                }
                // Deliberately just swallowing any error, since if we cannot get a token this way, then we use another fallback method.
                catch (FailureCallbackException) { }
            }

            if (null == result)
            {
                try
                {
                    result = await this.AcquireTokenInteractiveAsync(loginHint: loginHint, scopes: scopes, prompt: prompt);
                }
                catch (FailureCallbackException) { }
            }

            return result;
        }

        /// <summary>
        /// Acquires a token interactively asking the user for input. Depending on your application's configuration, the token is acquired either using
        /// a popup dialog, or by redirecting the user to the login.
        /// </summary>
        /// <param name="loginHint">A login hint to use, i.e. the username, if known.</param>
        /// <param name="scopes">
        /// The scopes that must be included in the acquired token. If not specified, the default configured scopes will be used.
        /// </param>
        /// <param name="prompt">The prompt behaviour to use. If not specified, no specific prompt behaviour is used. It will be determined by what is needed.</param>
        public async Task<AuthenticationResult> AcquireTokenInteractiveAsync(string loginHint = null, IEnumerable<string> scopes = null, LoginPrompt? prompt = null)
        {
            if(this.Options.InteractiveLoginMode == InteractiveLoginMode.Popup)
            {
                return await this.AcquireTokenPopupAsync(loginHint: loginHint, scopes: scopes, prompt: prompt);
            }
            else if(this.Options.InteractiveLoginMode == InteractiveLoginMode.Redirect)
            {
                await this.AcquireTokenRedirectAsync(loginHint: loginHint, scopes: scopes, prompt: prompt);
            }

            return null;
        }

        /// <summary>
        /// Acquires a token silently without user interaction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the application is configured to use <see cref="InteractiveLoginMode.Redirect"/>, then this method will also
        /// attempt to call <see cref="HandleRedirectPromiseAsync"/> in case the method is invoked on a page that the user
        /// was redirected to back from the identity provider.
        /// </para>
        /// <para>
        /// No interactive login is ever invoked by this method.
        /// </para>
        /// </remarks>
        /// <param name="loginHint">A login hint to use, i.e. the username, if known.</param>
        /// <param name="scopes">
        /// The scopes that must be included in the acquired token. If the user has not consented to one or more of these tokens, 
        /// the user will be taken to the interactive mode. If not specified, the default configured scopes will be used.
        /// </param>
        /// <param name="fallbackToDefaultLoginHint">
        /// Specifies whether to fall back to the default login hint. The default login hint is the login hint that was previously used to
        /// acquire a token.
        /// </param>
        public async Task<AuthenticationResult> AcquireTokenSilentAsync(string loginHint = null, IEnumerable<string> scopes = null, bool fallbackToDefaultLoginHint = false)
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
                var data = this.CreateMsalData(loginHint: loginHint, scopes: scopes, fallbackToDefaultLoginHint: fallbackToDefaultLoginHint);
                using(var handler = new DotNetInstanceCallbackHandler<AuthenticationResult>(module, "acquireTokenSilent", data))
                {
                    result = await handler.GetResultAsync();
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the default login hint for the current user. The default login hint is the login hint that was previously used to acquire a token.
        /// </summary>
        public async Task<string> GetDefaultLoginHintAsync()
        {
            var module = await this.GetBlazoradeModuleAsync();
            var data = this.CreateMsalData();

            string result = null;
            using (var handler = new DotNetInstanceCallbackHandler<string>(module, "getDefaultLoginHint", data))
            {
                result = await handler.GetResultAsync();
            }

            return result;
        }

        /// <summary>
        /// Assumes that the current request is a redirect back from the identity provider, and attempt to process information sent back to the
        /// application from the identity provider.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if the current request is not a redirect back from the identity provider.
        /// </remarks>
        /// <exception cref="FailureCallbackException">The exception that is thrown if the current request is a redirect back from login, but the redirect specifies an error with the login.</exception>
        public async Task<AuthenticationResult> HandleRedirectPromiseAsync()
        {
            var module = await this.GetBlazoradeModuleAsync();
            var data = this.CreateMsalData(navigateToLoginRequestUrl: false);

            AuthenticationResult result = null;
            using (var handler = new DotNetInstanceCallbackHandler<AuthenticationResult>(module, "handleRedirectPromise", data))
            {
                result = await handler.GetResultAsync();
            }
            return result;
        }

        /// <summary>
        /// Performs a logout of the current user.
        /// </summary>
        public async Task LogoutAsync()
        {
            var module = await this.GetBlazoradeModuleAsync();
            var data = this.CreateMsalData();

            using (var handler = new DotNetInstanceCallbackHandler(module, "logout", data))
            {
                await handler.GetResultAsync();
            }
        }



        /// <summary>
        /// Acquires a token with a popup dialog.
        /// </summary>
        /// <param name="loginHint">A login hint to use, i.e. the username, if known.</param>
        /// <param name="scopes">
        /// The scopes that must be included in the acquired token. If not specified, the default configured scopes will be used.
        /// </param>
        /// <param name="prompt">The prompt behaviour to use. If not specified, no specific prompt behaviour is used. It will be determined by what is needed.</param>
        /// <remarks>
        /// It is recommended to use the <see cref="AcquireTokenInteractiveAsync"/> method, which will determine the interaction mode from the application's configuration.
        /// </remarks>
        protected async Task<AuthenticationResult> AcquireTokenPopupAsync(string loginHint = null, IEnumerable<string> scopes = null, LoginPrompt? prompt = null)
        {
            var module = await this.GetBlazoradeModuleAsync();
            var data = this.CreateMsalData(loginHint: loginHint, scopes: scopes, prompt: prompt);

            AuthenticationResult result = null;
            using (var handler = new DotNetInstanceCallbackHandler<AuthenticationResult>(module, "acquireTokenPopup", data))
            {
                result = await handler.GetResultAsync();
            }
            return result;
        }

        /// <summary>
        /// Acquires a token by redirecting the user to the identity provider.
        /// </summary>
        /// <param name="loginHint">A login hint to use, i.e. the username, if known.</param>
        /// <param name="scopes">
        /// The scopes that must be included in the acquired token. If not specified, the default configured scopes will be used.
        /// </param>
        /// <param name="prompt">The prompt behaviour to use. If not specified, no specific prompt behaviour is used. It will be determined by what is needed.</param>
        /// <remarks>
        /// It is recommended to use the <see cref="AcquireTokenInteractiveAsync"/> method, which will determine the interaction mode from the application's configuration.
        /// </remarks>
        protected async Task AcquireTokenRedirectAsync(string loginHint = null, IEnumerable<string> scopes = null, LoginPrompt? prompt = null)
        {
            var module = await this.GetBlazoradeModuleAsync();
            var data = this.CreateMsalData(loginHint: loginHint, scopes: scopes, prompt: prompt);

            using (var handler = new DotNetInstanceCallbackHandler(module, "acquireTokenRedirect", data))
            {
                await handler.GetResultAsync();
            }
        }



        private Dictionary<string, object> CreateMsalConfig(bool navigateToLoginRequestUrl = true)
        {

            #region auth

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

            #endregion

            #region cache

            var cache = new Dictionary<string, object>
            {
                { "cacheLocation", "sessionStorage" }
            };
            if(this.Options.TokenCacheScope == TokenCacheScope.Persistent)
            {
                cache["cacheLocation"] = "localStorage";
            }

            #endregion

            var msalConfig = new Dictionary<string, object>
            {
                { "auth", auth },
                { "cache", cache }
            };
            return msalConfig;
        }

        private Dictionary<string, object> CreateMsalData(string loginHint = null, IEnumerable<string> scopes = null, bool navigateToLoginRequestUrl = true, bool fallbackToDefaultLoginHint = false, LoginPrompt? prompt = null)
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

            if (fallbackToDefaultLoginHint)
            {
                data["fallbackToDefaultLoginHint"] = true;
            }

            if (prompt.HasValue)
            {
                data["prompt"] = prompt.ToStringValue();
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
