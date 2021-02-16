using Blazorade.Msal.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Services
{
    /// <summary>
    /// A request object for acquiring tokens.
    /// </summary>
    /// <remarks>
    /// Not all token acquisition methods support all of the properties on this class. Properties that are not supported are ignored.
    /// </remarks>
    public class TokenAcquisitionRequest
    {
        /// <summary>
        /// Creates a new instance of the request class.
        /// </summary>
        public TokenAcquisitionRequest()
        {
        }


        /// <summary>
        /// Specifies whether to fall back to the default login hint. The default login hint is the login hint that
        /// was previously used to acquire a token.
        /// </summary>
        /// <remarks>
        /// This is used in certain cases where the login hint is not known or available.
        /// </remarks>
        public bool FallbackToDefaultLoginHint { get; set; }

        /// <summary>
        /// The number of milliseconds to specify as timeout for token acquisition calls.
        /// </summary>
        /// <remarks>
        /// If the timeout is <c>null</c>, a default value will be determined based on the type of token request. The timeout
        /// for interactive token requests is always longer than for silent requests.
        /// </remarks>
        public int? Timeout { get; set; }

        /// <summary>
        /// The username to acquire the token for. If the token cannot be acquired silently, this value will also be
        /// passed to the interactive method to minimize the amount of information the user has to enter.
        /// </summary>
        public string LoginHint { get; set; }

        /// <summary>
        /// The prompt behaviour to use. If not specified, no specific prompt behaviour is used.
        /// </summary>
        public LoginPrompt? Prompt { get; set; }

        /// <summary>
        /// The scopes to include in the requested access token. If not specified, the default scopes configured on
        /// the application is used.
        /// </summary>
        public IEnumerable<string> Scopes { get; set; }

        /// <summary>
        /// The key to the configured option to use for acquiring a token. The default options are used if not specified.
        /// </summary>
        public string OptionKey { get; set; }

    }
}
