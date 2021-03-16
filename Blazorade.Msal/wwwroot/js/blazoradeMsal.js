
export function acquireTokenSilent(args) {
    console.debug("acquireTokenSilent", args);

    let msalClient = createMsalClient(args);
    console.debug("acquireTokenSilent", "msalClient", msalClient);

    let account = msalClient.getAccountByUsername(args.data.loginHint);

    if (!account && args.data.fallbackToDefaultLoginHint) {
        console.debug("acquireTokenSilent", "Provided login hint did not find an account. Falling back to previously stored default login hint.");
        account = getDefaultAccount(args, msalClient);
    }

    if (!account) {
        // If we still don't have an account, then we need to log a warning.
        console.warn("acquireTokenSilent", "loginHint did not find an account. Tokens can most likely not be acquired without a found account.");
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
    if (args.data.prompt) request["prompt"] = args.data.prompt;
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
    if (args.data.prompt) request["prompt"] = args.data.prompt;
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

export function getDefaultLoginHint(args) {
    console.debug("getDefaultLoginHint", args);

    try {
        let loginHint = getDefaultLoginHintInternal(args);
        console.debug("getDefaultLoginHint", "loginHint", loginHint);
        invokeCallback(args.successCallback, loginHint);
    }
    catch (err) {
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
        invokeCallback(args.failureCallback, err);
    }
}

export function logout(args) {
    console.debug("logout", args);

    clearDefaultLoginHint(args);

    let msalClient = createMsalClient(args);
    let request = {};
    let logoutUrl = getLogoutUrl(args);
    if (logoutUrl) {
        request["postLogoutRedirectUri"] = logoutUrl;
    }

    console.debug("logout", "request", request);

    try {
        msalClient.logout(request);
        invokeCallback(args.successCallback);
    }
    catch (err) {
        invokeCallback(args.failureCallback, err);
    }
}



function clearDefaultLoginHint(args) {
    console.debug("clearDefaultLoginHint", args);
    let key = createDefaultLoginHintKey(args);
    window.localStorage.removeItem(key);
}

function createDefaultLoginHintKey(args) {
    return `${args.data.msalConfig.auth.clientId}.blazorade-default-loginHint`;
}

function createMsalClient(args) {
    console.debug("createMsalClient", args);
    setMsalConfigDefault(args.data.msalConfig);

    let msalClient = new msal.PublicClientApplication(args.data.msalConfig);
    console.debug("createMsalClient", "msalClient", msalClient);

    return msalClient;
}

function getDefaultAccount(args, msalClient) {
    console.debug("getDefaultAccount", args, msalClient);

    let account;
    let loginHint = getDefaultLoginHintInternal(args);
    console.debug("getDefaultAccount", "Default login hint", loginHint);

    if (loginHint) {
        account = msalClient.getAccountByUsername(loginHint);
        console.debug("getDefaultAccount", "Using default account", account);
    }

    return account;
}

function getDefaultLoginHintInternal(args) {
    console.debug("getDefaultLoginHintInternal", args);

    let key = createDefaultLoginHintKey(args);
    console.debug("getDefaultLoginHintInternal", "key", key);

    let loginHint = window.localStorage.getItem(key);
    console.debug("getDefaultLoginHintInternal", "loginHint", loginHint);

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
    console.debug("setDefaultLoginHint", args, authResult);

    if (authResult && authResult.account && authResult.account.username) {
        console.debug("setDefaultLoginHint", authResult.account.username);

        let key = createDefaultLoginHintKey(args);
        window.localStorage.setItem(key, authResult.account.username);
        console.debug("key", key);
    }
}

function setMsalConfigDefault(msalConfig) {
    console.debug("setMsalConfigDefault", msalConfig);

    msalConfig.auth = msalConfig.auth ? msalConfig.auth : {};
    msalConfig.auth.redirectUri = msalConfig.auth.redirectUri ? msalConfig.auth.redirectUri : window.location.origin;
}
