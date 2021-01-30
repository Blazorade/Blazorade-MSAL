using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Components
{
    partial class RedirectLoginHandler
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var result = await this.msalService.HandleRedirectPromiseAsync();
            }
        }
    }
}
