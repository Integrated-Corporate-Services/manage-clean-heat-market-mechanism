using Desnz.Chmm.Web.Middlewares;

namespace Desnz.Chmm.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseXsrfToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<XsrfTokenMiddleware>();
        }

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }

        public static IApplicationBuilder UseGoogleAnalytics(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GoogleAnalyticsMiddleware>();
        }
    }
}
