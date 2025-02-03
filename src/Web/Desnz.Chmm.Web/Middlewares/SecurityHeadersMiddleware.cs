using Desnz.Chmm.Web.Constants;
using System.Security.Cryptography;

namespace Desnz.Chmm.Web.Middlewares
{
    internal class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var scriptNonce = GenerateNonce();
            context.Items.Add(SecurityHeadersConstants.HttpContextItemsKey.NONCE, scriptNonce);
            context.Response.Headers.Append(
                "Content-Security-Policy",
                $"default-src 'self'; style-src 'self' 'nonce-{scriptNonce}'; script-src 'self' 'nonce-{scriptNonce}'; connect-src 'self' https://*.google-analytics.com; frame-ancestors 'none'");

            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            await _next(context);
        }

        private string? GenerateNonce()
        {
            string? nonce = null;
            using (var rng = RandomNumberGenerator.Create())
            {
                var nonceBytes = new byte[32];
                rng.GetBytes(nonceBytes);
                nonce = Convert.ToBase64String(nonceBytes);
            }
            return nonce;
        }
    }
}
