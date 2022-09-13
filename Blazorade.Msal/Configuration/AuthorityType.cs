using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Configuration
{
    /// <summary>
    /// Defines different authority types that Blazorade MSAL support.
    /// </summary>
    public enum AuthorityType
    {
        /// <summary>
        /// Azure AD. Only the <see cref="BlazoradeMsalOptions.ClientId"/> and <see cref="BlazoradeMsalOptions.TenantId"/> are required
        /// to be configured.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the default authority type used by the <see cref="BlazoradeMsalOptions"/> configuration options class.
        /// </para>
        /// <para>
        /// You can override this default behaviour by setting <see cref="BlazoradeMsalOptions.Authority"/> to a full URL in the form
        /// of https://login.microsoftonline.com/&lt;tenant ID&gt;
        /// </para>
        /// </remarks>
        AAD,

        /// <summary>
        /// Azure AD B2C. With this option, you need to specify at least the following options:
        /// <see cref="BlazoradeMsalOptions.ClientId"/>, <see cref="BlazoradeMsalOptions.TenantId"/> and <see cref="BlazoradeMsalOptions.EndpointId"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// You can override the default behaviour by setting the <see cref="BlazoradeMsalOptions.Authority"/> to a full URL in the form
        /// of https://&lt;Tenant Name&gt;.b2clogin.com/&lt;Tenant Name&gt;.onmicrosoft.com/&lt;Policy ID&gt;
        /// </para>
        /// </remarks>
        AADB2C,

        /// <summary>
        /// A generic authority that supports authentication with Open ID Connect.
        /// </summary>
        /// <remarks>
        /// When you use this options, you must specify the full URL for the <see cref="BlazoradeMsalOptions.Authority"/> configuration option.
        /// </remarks>
        OidcGeneric
        
    }
}
