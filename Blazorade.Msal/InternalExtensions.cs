using Blazorade.Msal.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal
{
    internal static class InternalExtensions
    {

        public static string ToStringValue(this LoginPrompt? prompt)
        {
            switch (prompt)
            {
                case LoginPrompt.Consent:
                    return "consent";

                case LoginPrompt.Login:
                    return "login";

                case LoginPrompt.None:
                    return "none";

                case LoginPrompt.SelectAccount:
                    return "select_account";

                default:
                    return null;
            }
        }
    }
}
