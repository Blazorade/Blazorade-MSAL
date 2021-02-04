using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Configuration
{
    /// <summary>
    /// Specifies how tokens are cached.
    /// </summary>
    public enum TokenCacheScope
    {
        /// <summary>
        /// Tokens are cached for a single browser session.
        /// </summary>
        /// <remarks>
        /// May result in that tokens are not shared across multiple browser tabs.
        /// </remarks>
        Session,

        /// <summary>
        /// Tokens are persisted across browser sessions.
        /// </summary>
        Persistent
    }
}
