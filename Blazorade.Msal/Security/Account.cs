using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Security
{
    public sealed class Account
    {
        public string AccountIdentifier { get; set; }

        public string HomeAccountIdentifier { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public Dictionary<string, object> IdTokenClaims { get; set; }

        public string Sid { get; set; }

        public string Environment { get; set; }

    }
}
