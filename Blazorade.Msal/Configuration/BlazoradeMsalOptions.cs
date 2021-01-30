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
            this.MsalVersion = "2.8.0";
            this.InteractiveLoginMode = InteractiveLoginMode.DefaultDialog;
        }


        public string ClientId { get; set; }

        public string TenantId { get; set; }

        public string MsalVersion { get; set; }

        public InteractiveLoginMode InteractiveLoginMode { get; set; }

    }
}
