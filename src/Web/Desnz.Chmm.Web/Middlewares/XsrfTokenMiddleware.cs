using Microsoft.AspNetCore.Antiforgery;

namespace Desnz.Chmm.Web.Middlewares
{
    internal class XsrfTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAntiforgery _antiforgery;

        public XsrfTokenMiddleware(RequestDelegate next, IAntiforgery antiforgery)
        {
            _next = next;
            _antiforgery = antiforgery;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestPath = context.Request.Path.Value;

            var tokenSet = _antiforgery.GetAndStoreTokens(context);
            context.Response.Cookies.Append(
                "XSRF-TOKEN",
            tokenSet.RequestToken!,
                new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.Strict, Secure = true }
            );

            await _next(context);
        }
    }
}
