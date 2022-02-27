using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Services
{
    /// <summary>
    /// A request factory implementation that can create HTTP requests that are authorized with
    /// an access token provided by <see cref="BlazoradeMsalService"/>.
    /// </summary>
    public class BlazoradeRequestFactory
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="msalService">The service dependency to inject.</param>
        /// <exception cref="ArgumentNullException">The exception that is thrown if <paramref name="msalService"/> is <c>null</c>.</exception>
        public BlazoradeRequestFactory(BlazoradeMsalService msalService)
        {
            this.MsalService = msalService ?? throw new ArgumentNullException(nameof(msalService));
        }

        private readonly BlazoradeMsalService MsalService;

        /// <summary>
        /// Creates a request with the given options.
        /// </summary>
        /// <param name="options">The options that control how the request will be created and how the access token is acquired.</param>
        /// <exception cref="ArgumentNullException">The exception that is thrown if <paramref name="options"/> is <c>null</c>.</exception>
        public async Task<HttpRequestMessage> CreateRequestAsync(CreateRequestOptions options)
        {
            if (null == options) throw new ArgumentNullException(nameof(options));

            var request = new HttpRequestMessage(options.Method, options.RequestUri);

            var authResult = await this.MsalService.AcquireTokenAsync(
                loginHint: options.LoginHint, 
                scopes: options.Scopes, 
                fallbackToDefaultLoginHint: options.FallbackToDefaultLoginHint, 
                prompt: options.Prompt
            );
            if(authResult?.AccessToken?.Length > 0)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            }

            return request;
        }

        /// <summary>
        /// Creates a request to the specified <paramref name="requestUri"/> with the specified <paramref name="method"/> 
        /// which is authorized with an access token for the given <paramref name="scopes"/>.
        /// </summary>
        /// <param name="requestUri">The URI for the request.</param>
        /// <param name="method"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public async Task<HttpRequestMessage> CreateRequestAsync(Uri requestUri, HttpMethod method, params string[] scopes)
        {
            var options = new CreateRequestOptions(requestUri)
            {
                Method = method,
                Scopes = scopes
            };

            return await this.CreateRequestAsync(options);
        }

        public async Task<HttpRequestMessage> CreateRequestAsync(string requestUri, HttpMethod method, params string[] scopes)
        {
            return await this.CreateRequestAsync(new Uri(requestUri, UriKind.RelativeOrAbsolute), method, scopes);
        }

        public async Task<HttpRequestMessage> CreateDeleteRequestAsync(Uri requestUri, params string[] scopes)
        {
            return await this.CreateRequestAsync(requestUri, HttpMethod.Delete, scopes);
        }

        public async Task<HttpRequestMessage> CreateDeleteRequestAsync(string requestUri, params string[] scopes)
        {
            return await this.CreateRequestAsync(requestUri, HttpMethod.Delete, scopes);
        }

        public async Task<HttpRequestMessage> CreateGetRequestAsync(Uri requestUri, params string[] scopes)
        {
            return await this.CreateRequestAsync(requestUri, HttpMethod.Get, scopes);
        }

        public async Task<HttpRequestMessage> CreateGetRequestAsync(string requestUri, params string[] scopes)
        {
            return await this.CreateRequestAsync(requestUri, HttpMethod.Get, scopes);
        }

        public async Task<HttpRequestMessage> CreatePatchRequestAsync(Uri requestUri, params string[] scopes)
        {
            return await this.CreateRequestAsync(requestUri, HttpMethod.Patch, scopes);
        }

        public async Task<HttpRequestMessage> CreatePatchRequestAsync(string requestUri, params string[] scopes)
        {
            return await this.CreateRequestAsync(requestUri, HttpMethod.Patch, scopes);
        }

        public async Task<HttpRequestMessage> CreatePostRequestAsync(Uri requestUri, params string[] scopes)
        {
            return await this.CreateRequestAsync(requestUri, HttpMethod.Post, scopes);
        }

        public async Task<HttpRequestMessage> CreatePostRequestAsync(string requestUri, params string[] scopes)
        {
            return await this.CreateRequestAsync(requestUri, HttpMethod.Post, scopes);
        }

        public async Task<HttpRequestMessage> CreatePutRequestAsync(Uri requestUri, params string[] scopes)
        {
            return await this.CreateRequestAsync(requestUri, HttpMethod.Put, scopes);
        }

        public async Task<HttpRequestMessage> CreatePutRequestAsync(string requestUri, params string[] scopes)
        {
            return await this.CreateRequestAsync(requestUri, HttpMethod.Put, scopes);
        }
    }
}
