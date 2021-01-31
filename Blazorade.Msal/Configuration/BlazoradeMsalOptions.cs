using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Configuration
{
    public class BlazoradeMsalOptions
    {

        public BlazoradeMsalOptions()
        {
            this.MsalVersion = "2.9.0";
            this.InteractiveLoginMode = InteractiveLoginMode.Dialog;
            this.DefaultScopes = new string[] { "openid", "profile" };
        }


        public string ClientId { get; set; }

        public string TenantId { get; set; }

        public string MsalVersion { get; set; }

        public InteractiveLoginMode InteractiveLoginMode { get; set; }

        public IEnumerable<string> DefaultScopes { get; set; }

        public string RedirectUrl { get; set; }

        public string PostLogoutUrl { get; set; }

    }
}
