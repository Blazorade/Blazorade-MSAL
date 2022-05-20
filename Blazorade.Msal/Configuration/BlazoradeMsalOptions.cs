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
    public class BlazoradeMsalOptions
    {

        /// <summary>
        /// Creates a new instance of the class and sets the defaults.
        /// </summary>
        public BlazoradeMsalOptions()
        {
            this.MsalVersion = "2.11.0";
            this.InteractiveLoginMode = InteractiveLoginMode.Redirect;
            this.DefaultScopes = new string[] { "openid", "profile" };
            this.TokenCacheScope = TokenCacheScope.Session;

            this.AuthorityType = AuthorityType.AAD;
        }

        /// <summary>
        /// The client ID representing your application.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The name or ID of the endpoint specified by the configuration options.
        /// </summary>
        /// <remarks>
        /// For instance with Azure AD B2C, this is the ID of the policy you want to use for users when signing in.
        /// </remarks>
        public string EndpointId { get; set; }

        /// <summary>
        /// The tenant ID in which the application is registered. 
        /// </summary>
        /// <remarks>
        /// The tenant ID can either be a Guid, the default domain, i.e. <c>[your tenant].onmicrosoft.com</c>,
        /// or a vanity domain like <c>yourcompany.com</c>. If your application is a multi-tenant application,
        /// you can also set the tenant to <c>common</c>, <c>organizations</c> or <c>consumers</c>.
        /// </remarks>
        public string TenantId { get; set; }

        /// <summary>
        /// The full authority URI representing the Azure AD tenant that will take care of authenticating your users. If this
        /// property is set, <see cref="TenantId"/> is ignored.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <c>TenantId</c> option must always be set to a value that can be resolved by Azure AD. This means that it has
        /// to be formatted as one of the following.
        /// <list type="bullet">
        /// <item>
        /// <term>Default domain name</term>
        /// <description>
        /// The default domain name is formatted as <c>&lt;tenant name&gt;.onmicrosoft.com</c>. With Azure AD B2C, this
        /// is the only supported option.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Tenant GUID</term>
        /// <description>You can specify the tenant as the directory GUID.</description>
        /// </item>
        /// <item>
        /// <term>Vanity domain</term>
        /// <description>
        /// You can also use your vanity domain to define your tenant.
        /// </description>
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// The <c>Authority</c> property allows you to use both Azure AD and Azure AD B2C. Depending on which directory
        /// you are using, the value is constructed a bit differently.
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <term>Azure AD</term>
        /// <description>https://login.microsoftonline.com/{tenant-name}.onmicrosoft.com</description>
        /// </item>
        /// <item>
        /// <term>Azure AD B2C</term>
        /// <description>https://{tenant-name}.b2clogin.com/{tenant-name}.onmicrosoft.com/{policy-name}</description>
        /// </item>
        /// </list>
        /// </remarks>
        public string Authority { get; set; }

        /// <summary>
        /// The type of authority to use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Using this configuration option helps you in formatting the <see cref="Authority"/> URL to use when logging in.
        /// </para>
        /// <para>
        /// You can always override this setting by explicitly setting the <see cref="Authority"/> URL in your
        /// configuration options.
        /// </para>
        /// <para>
        /// The default value is <see cref="AuthorityType.AAD"/>.
        /// </para>
        /// </remarks>
        public AuthorityType AuthorityType { get; set; }

        /// <summary>
        /// The MSAL Browser version to use.
        /// </summary>
        /// <remarks>
        /// Defaults to 2.22.0. For more information see https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-browser.
        /// </remarks>
        public string MsalVersion { get; set; } = "2.22.0";

        /// <summary>
        /// Defines how interactive login is handled.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="InteractiveLoginMode.Redirect"/>.
        /// </remarks>
        public InteractiveLoginMode InteractiveLoginMode { get; set; } = InteractiveLoginMode.Redirect;

        /// <summary>
        /// The default scopes to acquire if none are specified when acquiring tokens.
        /// </summary>
        /// <remarks>
        /// Defaults to an array with <c>openid</c> and <c>profile</c>.
        /// </remarks>
        public IEnumerable<string> DefaultScopes { get; set; } = new string[] { "openid", "profile" };

        /// <summary>
        /// The redirect URI for your application.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the URL where your users are redirected back after interactive login if <see cref="InteractiveLoginMode"/>
        /// is set to <see cref="InteractiveLoginMode.Redirect"/>.
        /// </para>
        /// <para>
        /// Can be set to an absolute or relative URI.
        /// </para>
        /// </remarks>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// The URI that the users are redirected to after logging out of your application.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Can be set to an absolute or relative URI.
        /// </para>
        /// </remarks>
        public string PostLogoutUrl { get; set; }

        /// <summary>
        /// Defines how tokens are cached.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TokenCacheScope.Session"/>.
        /// </remarks>
        public TokenCacheScope TokenCacheScope { get; set; } = TokenCacheScope.Session;



        internal string GetAuthority()
        {
            return this.Authority?.Length > 0
                ? this.Authority
                : this.BuildAuthority();
        }


        private string BuildAuthority()
        {
            string url = null;

            if(this.AuthorityType == AuthorityType.AAD)
            {
                url = $"https://login.microsoftonline.com/{ this.TenantId ?? "common" }";
            }
            else if (this.AuthorityType == AuthorityType.AADB2C)
            {
                var segments = this.TenantId.Split('.');
                if(segments.Length < 3)
                {
                    throw new Exception("The TenantId configuration option must be specified as '<Tenant name>.onmicrosoft.com' for Azure AD B2C.");
                }
                if(string.IsNullOrEmpty(this.EndpointId))
                {
                    throw new Exception("You must set the EndpointId configuration option to the ID of the policy you want to use to log in with in Azure AD B2C.");
                }

                url = $"https://{segments[0]}.b2clogin.com/{this.TenantId}/{this.EndpointId}";
            }
            else
            {
                throw new Exception($"Cannot build the Authority URL for this kind of authority type: '{this.AuthorityType}'.");
            }

            return url;
        }
    }
}
