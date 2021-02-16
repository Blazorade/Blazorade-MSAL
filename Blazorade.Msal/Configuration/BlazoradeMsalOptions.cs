using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Configuration
{
    /// <summary>
    /// Contains options for customizing how your application uses MSAL.
    /// </summary>
    public class BlazoradeMsalOptions : ApplicationConfigurationOptions
    {
        public BlazoradeMsalOptions()
        {
            this.AlternativeOptions = new Dictionary<string, ApplicationConfigurationOptions>();
        }

        private Dictionary<string, ApplicationConfigurationOptions> _AlternativeOptions;
        public Dictionary<string, ApplicationConfigurationOptions> AlternativeOptions
        {
            get => _AlternativeOptions;
            set => _AlternativeOptions = value ?? new Dictionary<string, ApplicationConfigurationOptions>();
        }

    }
}
