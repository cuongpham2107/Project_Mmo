
var config2 = {
    mode: "fixed_servers",
    rules: {
        singleProxy: {
            scheme: 'http',
            host: '{host}',
            port: {port}
        },
        bypassList: []
    }
}
function proxyRequest(request_data) {
    return {
        type: 'http',
        host: '{host}',
        port: {port}
    };
}
chrome.proxy.settings.set({ value: config2, scope: "regular" }, function () { });
function callbackFn(details) { return { authCredentials: { username: '{username}', password: '{password}' } } };
chrome.webRequest.onAuthRequired.addListener(
    callbackFn,
    { urls: ["<all_urls>"] },
    ['blocking']
);
chrome.proxy.onRequest.addListener(proxyRequest, { urls: ["<all_urls>"] })