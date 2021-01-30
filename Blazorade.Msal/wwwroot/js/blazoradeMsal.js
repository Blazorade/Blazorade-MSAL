
export function acquireTokenSilent(args) {
    console.debug("acquireTokenSilent", args);

    let msalClient = createMsalClient(args);
    console.debug("acquireTokenSilent", "msalClient", msalClient);

    let account = msalClient.getAccountByUsername(args.data.loginHint);

    if (!account) {
        console.warn("acquireTokenSilent", "loginHint did not find an account. Falling back to current account.");
        let accounts = msalClient.getAllAccounts();
        if (accounts && accounts.length) account = accounts[0];
    }

    console.debug("acquireTokenSilent", "account", account);

    let request = {
        account: account,
        scopes: getScopes(args),
        authority: args.data.msalConfig.auth.authority
    };
    console.debug("acquireTokenSilent", "request", request);

    try {
        msalClient.acquireTokenSilent(request)
            .then(result => {
                console.debug("acquireTokenSilent", "success", result);
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
        scopes: getScopes(args),
        authority: args.data.msalConfig.auth.authority,
        loginHint: args.data.loginHint
    };
    console.debug("acquireTokenPopup", "request", request);

    try {
        msalClient.acquireTokenPopup(request)
            .then(result => {
                console.debug("acquireTokenPopup", "success", result);
                invokeCallback(args.successCallback, result);
            })
            .catch(err => {
                console.error("acquireTokenPopup", "failure", err);
                invokeCallback(args.failureCallback, err);
            });
    }
    catch (err) {
        console.error("acquireTokenPopup", "Failure calling MSAL client", err);
    }
}

function createMsalClient(args) {
    console.debug("createMsalClient", args);
    setMsalConfigDefault(args.data.msalConfig);

    let msalClient = new msal.PublicClientApplication(args.data.msalConfig);
    console.debug("createMsalClient", "msalClient", msalClient);

    return msalClient;
}

function getScopes(args) {
    if (args && args.data && args.data.scopes) {
        return args.data.scopes;
    }

    return [".default"];
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

function setMsalConfigDefault(msalConfig) {
    console.debug("setMsalConfigDefault", msalConfig);

    msalConfig.auth.redirectUri = window.location.origin;
    msalConfig.cache = {
        cacheLocation: "localStorage"
    };
}
