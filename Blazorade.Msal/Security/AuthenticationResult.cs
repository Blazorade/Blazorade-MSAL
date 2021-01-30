using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Security
{
    public sealed class AuthenticationResult
    {
        public AuthenticationResult()
        {
            this.IdTokenClaims = new Dictionary<string, object>();
        }

        public string Authority { get; set; }

        public string UniqueId { get; set; }

        public string TenantId { get; set; }

        public string TokenType { get; set; }

        public string IdToken { get; set; }

        public Dictionary<string, object> IdTokenClaims { get; set; }

        public string AccessToken { get; set; }

        public List<string> Scopes { get; set; }

        public DateTimeOffset ExpiresOn { get; set; }

        public Account Account { get; set; }

        public string AccountState { get; set; }

        public bool FromCache { get; set; }

    }
}
