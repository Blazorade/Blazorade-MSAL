using Blazorade.Msal.Security;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Components
{
    partial class LoginRedirectHandler
    {

        [Parameter]
        public EventCallback<AuthenticationResult> OnLoginSucceeded { get; set; }

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
