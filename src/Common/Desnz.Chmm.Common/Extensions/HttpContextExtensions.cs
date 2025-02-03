using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Desnz.Chmm.Common.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid? GetOrganisationId(this HttpContext httpContext)
        {
            var organisationId = httpContext.GetOrganisationIdFromRouteData();
            if (organisationId == null)
            {
                organisationId = httpContext.GetOrganisationIdFromBody();
                if (organisationId == null)
                {
                    organisationId = httpContext.GetOrganisationIdFromForm();
                }
            }
            return organisationId;
        }

        public static Guid? GetOrganisationIdFromRouteData(this HttpContext httpContext) => 
            Guid.TryParse(httpContext.GetRouteValue("organisationId")?.ToString(), out var guid)
                ? guid
                : null;

        public static Guid? GetOrganisationIdFromBody(this HttpContext httpContext)
        {
            var request = httpContext.Request;
            request.EnableBuffering();

            using var streamReader = new StreamReader(request.Body, leaveOpen: true);
            var requestBody = Task.Run(() => streamReader.ReadToEndAsync()).Result;
            Guid? organisationId = null;

            try
            {
                var requestBodyObject = JObject.Parse(requestBody);
                if (requestBodyObject.TryGetValue("organisationId", StringComparison.OrdinalIgnoreCase, out var organisationIdToken) &&
                    Guid.TryParse(organisationIdToken.ToString(), out var guid))
                {
                    organisationId = guid;
                }
            }
            catch (JsonReaderException)
            {
            }

            request.Body.Position = 0;

            return organisationId;
        }

        public static Guid? GetOrganisationIdFromForm(this HttpContext httpContext) =>
            httpContext.Request.HasFormContentType && httpContext.Request.Form.TryGetValue("organisationId", out var sOrganisationId) &&
            Guid.TryParse(sOrganisationId, out var organisationId)
                ? organisationId 
                : null;
    }
}
