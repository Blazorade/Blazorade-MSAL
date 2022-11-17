using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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

        public IEnumerable<Claim> AccessTokenClaims 
            => ReadJwtToken(AccessToken);

        public string AccessToken { get; set; }

        public List<string> Scopes { get; set; }

        public DateTimeOffset? ExpiresOn { get; set; }

        public Account Account { get; set; }

        public bool FromCache { get; set; }

        private IEnumerable<Claim> ReadJwtToken(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                return Enumerable.Empty<Claim>();
            }

            try
            {
                return new JwtSecurityTokenHandler().ReadJwtToken(AccessToken).Claims;
            }
            catch (Exception)
            {
                return Enumerable.Empty<Claim>();
            }
        }
    }
}
