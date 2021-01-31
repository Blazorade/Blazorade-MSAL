
export function acquireTokenSilent(args) {
    console.debug("acquireTokenSilent", args);

    let msalClient = createMsalClient(args);
    console.debug("acquireTokenSilent", "msalClient", msalClient);

    let account = msalClient.getAccountByUsername(args.data.loginHint);

    if (!account) {
        console.warn("acquireTokenSilent", "loginHint did not find an account. Tokens can most likely not be acquired without a found account.");
        if (args.data.fallbackToDefaultAccount) {
            let loginHint = getDefaultLoginHint(args);
            console.debug("acquireTokenSilent", "fallback login hint", loginHint);

            if (loginHint) {
                account = msalClient.getAccountByUsername(loginHint);
                if (account) {
                    console.debug("acquireTokenSilent", "Fallback to default account", account);
                }
            }
        }
    }

    console.debug("acquireTokenSilent", "account", account);

    let request = {
        account: account,
        scopes: args.data.scopes,
        authority: args.data.msalConfig.auth.authority
    };
    console.debug("acquireTokenSilent", "request", request);

    try {
        msalClient.acquireTokenSilent(request)
            .then(result => {
                console.debug("acquireTokenSilent", "success", result);
                setDefaultLoginHint(args, result);
                invokeCallback(args.successCallback, result);
            }).catch(err => {
                console.warn("acquireTokenSilent", "failure", err);
                invokeCallback(args.failureCallback, err);
            });
    }
    catch (err) {
        console.error("acquireTokenSilent", "Failure calling MSAL client", err);
        invokeCallback(args.failureCallback, err);
    }
}

export function acquireTokenPopup(args) {
    console.debug("acquireTokenPopup", args);

    let msalClient = createMsalClient(args);
    console.debug("acquireTokenPopup", "msalClient", msalClient);

    let request = {
        scopes: args.data.scopes,
        authority: args.data.msalConfig.auth.authority,
        loginHint: args.data.loginHint
    };
    console.debug("acquireTokenPopup", "request", request);

    try {
        msalClient.acquireTokenPopup(request)
            .then(result => {
                console.debug("acquireTokenPopup", "success", result);
                setDefaultLoginHint(args, result);
                invokeCallback(args.successCallback, result);
            })
            .catch(err => {
                console.error("acquireTokenPopup", "failure", err);
                invokeCallback(args.failureCallback, err);
            });
    }
    catch (err) {
        console.error("acquireTokenPopup", "Failure calling MSAL client", err);
        invokeCallback(args.failureCallback, err);
    }
}

export function acquireTokenRedirect(args) {
    console.debug("acquireTokenRedirect", args);

    let msalClient = createMsalClient(args);
    console.debug("acquireTokenRedirect", "msalClient", msalClient);

    let request = {
        scopes: args.data.scopes,
        authority: args.data.msalConfig.auth.authority,
        loginHint: args.data.loginHint,
        redirectUri: args.data.msalConfig.auth.redirectUri
    };
    console.debug("acquireTokenRedirect", "request", request);

    try {
        msalClient.acquireTokenRedirect(request);
        invokeCallback(args.successCallback, null);
    }
    catch (err) {
        console.error("acquireTokenRedirect", "Failure calling MSAL client", err);
        invokeCallback(args.failureCallback, err);
    }
}

export function handleRedirectPromise(args) {
    console.debug("handleRedirectPromise", args);

    let msalClient = createMsalClient(args);
    console.debug("handleRedirectPromise", "msalClient", msalClient);

    try {
        msalClient.handleRedirectPromise()
            .then(result => {
                console.debug("handleRedirectPromise", "success", result);
                setDefaultLoginHint(args, result);
                invokeCallback(args.successCallback, result);
            })
            .catch(err => {
                console.error("handleRedirectPromise", "failure", err);
                invokeCallback(args.failureCallback, err);
            });
    }
    catch (err) {
        console.error("handleRedirectPromise", "Failure calling MSAL client", err);
    }
}

export function logout(args) {
    console.debug("logout", args);

    let msalClient = createMsalClient(args);
    let request = {};
    let logoutUrl = getLogoutUrl(args);
    if (logoutUrl) {
        request["postLogoutRedirectUri"] = logoutUrl;
    }

    console.debug("logout", "request", request);

    msalClient.logout(request);
}


function createMsalClient(args) {
    console.debug("createMsalClient", args);
    setMsalConfigDefault(args.data.msalConfig);

    let msalClient = new msal.PublicClientApplication(args.data.msalConfig);
    console.debug("createMsalClient", "msalClient", msalClient);

    return msalClient;
}

function getDefaultLoginHint(args) {
    console.debug("getDefaultLoginHint", args);

    let key = `${args.data.msalConfig.auth.clientId}.blazorade-default-loginHint`;
    console.debug("getDefaultLoginHint", "key", key);

    let loginHint = window.localStorage.getItem(key);
    console.log("getDefaultLoginHint", "loginHint", loginHint);
    return loginHint;
}

function getLogoutUrl(args) {

    if (args && args.msalConfig && args.msalConfig.auth) {
        return args.msalConfig.auth["postLogoutRedirectUri"];
    }
    return null;
}

function invokeCallback(callback, ...args) {
    console.debug("invokeCallback", callback, args);
    if (callback && callback.target && callback.methodName) {
        callback.target.invokeMethodAsync(callback.methodName, ...args);
    }
    else {
        console.error("invokeCallbck", "Given method reference cannot be used for invoking a callback.", callback, args);
    }
}

function setDefaultLoginHint(args, authResult) {
    console.debug("setDefaultAccount", args, authResult);

    if (authResult && authResult.account && authResult.account.username) {
        console.debug("setDefaultAccount", authResult.account.username);

        let key = `${args.data.msalConfig.auth.clientId}.blazorade-default-loginHint`;
        window.localStorage.setItem(key, authResult.account.username);
        console.debug("key", key);
    }
}

function setMsalConfigDefault(msalConfig) {
    console.debug("setMsalConfigDefault", msalConfig);

    msalConfig.auth = msalConfig.auth ?? {};
    msalConfig.auth.redirectUri = msalConfig.auth.redirectUri ?? window.location.origin;
    msalConfig.cache = {
        cacheLocation: "localStorage"
    };
}

