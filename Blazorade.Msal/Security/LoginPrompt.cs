using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Msal.Security
{
    /// <summary>
    /// Defines different prompt behaviour for interactive login.
    /// </summary>
    public enum LoginPrompt
    {
        /// <summary>
        /// Will force the user to enter their credentials on that request, negating single-sign on.
        /// </summary>
        Login,

        /// <summary>
        /// Will the trigger the OAuth consent dialog after the user signs in, asking the user to grant permissions to the app.
        /// </summary>
        Consent,

        /// <summary>
        /// Will interrupt single sign-on providing account selection experience listing all the accounts in session or any remembered accounts or an option to choose to use a different account.
        /// </summary>
        SelectAccount,

        /// <summary>
        /// Will ensure that the user isn't presented with any interactive prompt. if request can't be completed via single-sign on, the login process will fail.
        /// </summary>
        None
    }
}
