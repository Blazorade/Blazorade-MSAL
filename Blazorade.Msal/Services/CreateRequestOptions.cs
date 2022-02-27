using Blazorade.Msal.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Services
{
#nullable enable

    public class CreateRequestOptions
    {
        /// <summary>
        /// Creates a new class instance and specifies the <see cref="RequestUri"/>.
        /// </summary>
        /// <param name="requestUri">The URI for the request.</param>
        /// <exception cref="ArgumentNullException">The exception that is thrown if <paramref name="requestUri"/> is <c>null</c>.</exception>
        public CreateRequestOptions(Uri requestUri)
        {
            this.RequestUri = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
        }

        /// <summary>
        /// The URI for the request.
        /// </summary>
        /// <remarks>
        /// Can be either absolute or relative, but a relative URI requires that the base URI is set on the HTTP client.
        /// </remarks>
        public Uri RequestUri { get; set; }

        /// <summary>
        /// A collection of scopes required for the request.
        /// </summary>
        public IEnumerable<string> Scopes { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// The request method.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="HttpMethod.Get"/>.
        /// </remarks>
        public HttpMethod Method { get; set; } = HttpMethod.Get;

        /// <summary>
        /// A login hint to use when acquiring tokens.
        /// </summary>
        public string? LoginHint { get; set; }

        /// <summary>
        /// Specifies whether to use a previously used login hint if <see cref="LoginHint"/> is not specified.
        /// </summary>
        public bool FallbackToDefaultLoginHint { get; set; } = true;

        /// <summary>
        /// How to prompt the user to log in in case the user must log in to acquire the required token.
        /// </summary>
        public LoginPrompt? Prompt { get; set; }

    }
}
