using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Web.Constants;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Desnz.Chmm.Web.Middlewares
{
    internal class GoogleAnalyticsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly GoogleAnalyticsConfig _config;

        public GoogleAnalyticsMiddleware(RequestDelegate next, IOptions<GoogleAnalyticsConfig> config)
        {
            _next = next;
            _config = config.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Items.Add(GoogleAnalyticsConstants.HttpContextItemsKey.GoogleAnalyticsKey, _config.Key);

            await _next(context);
        }
    }
}
