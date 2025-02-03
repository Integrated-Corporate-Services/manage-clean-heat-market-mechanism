const PROXY_CONFIG = [
  {
    context: ["/signin-oidc", "/signout-callback-oidc", "/oidc", "/api"],
    proxyTimeout: 100000,
    target: "https://localhost:32776",
    secure: false,
    headers: {
      Connection: "Keep-Alive",
    },
  },
];

module.exports = PROXY_CONFIG;
