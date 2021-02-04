using Blazorade.Msal.Security;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Components
{
    /// <summary>
    /// Use this component on your login page if you are using redirect login in your application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The component needs to be added to the page that is configured as the redirect URI in your application,
    /// both in Azure AD and in the startup configuration for your application.
    /// </para>
    /// <para>
    /// The component will process the redirect when a user is redirected back from signing in. Depending on the outcome of the login process,
    /// either <see cref="OnLoginSucceeded"/> or <see cref="OnLoginFailed"/> event is triggered.
    /// </para>
    /// </remarks>
    partial class LoginRedirectHandler
    {

        /// <summary>
        /// The event that is triggered when the component has successfully processed a login redirect request. The argument with the event
        /// contains the result of the authentication.
        /// </summary>
        [Parameter]
        public EventCallback<AuthenticationResult> OnLoginSucceeded { get; set; }

        /// <summary>
        /// The event that is triggered when the login has failed. The argument with the event contains the exception.
        /// </summary>
        [Parameter]
        public EventCallback<Exception> OnLoginFailed { get; set; }



        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                AuthenticationResult token = null;

                try
                {
                    token = await this.msalService.HandleRedirectPromiseAsync();
                    if (null != token)
                    {
                        await this.OnLoginSucceeded.InvokeAsync(token);
                    }
                    else
                    {
                        await this.OnLoginFailed.InvokeAsync();
                    }
                }
                catch (Exception ex)
                {
                    await this.OnLoginFailed.InvokeAsync(ex);
                }
            }
        }
    }
}
