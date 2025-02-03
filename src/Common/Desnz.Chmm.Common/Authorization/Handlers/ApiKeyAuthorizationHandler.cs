using Desnz.Chmm.Common.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Desnz.Chmm.Common.Authorization.Handlers
{
    public class ApiKeyAuthorizationHandler : AuthorizationHandler<ApiKeyRequirement>
    {
        private readonly ILogger<ApiKeyAuthorizationHandler> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public ApiKeyAuthorizationHandler(
            ILogger<ApiKeyAuthorizationHandler> logger,
            IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ApiKeyRequirement requirement)
        {
            _logger.LogWarning("Evaluating authorization requirement for {requirementName}: {requirementValue}", nameof(ApiKeyRequirement), JsonConvert.SerializeObject(requirement));

            var requestPath = _contextAccessor.HttpContext?.Request.Path;

            var apiKey = _contextAccessor.HttpContext?.Request.Headers[requirement.HeaderName].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogInformation("No header {headerName} value present in request to {requestPath}", requirement.HeaderName, requestPath);
                context.Fail();
            }

            if (apiKey != requirement.ApiKey)
            {
                _logger.LogInformation("{requestPath} request header {headerName} value {headerValue} does not satisfy the {requirementName}: {requirementValue}",
                    requestPath, requirement.HeaderName, apiKey, nameof(ApiKeyRequirement), JsonConvert.SerializeObject(requirement));
                context.Fail();
            }

            context.Succeed(requirement);
        }
    }
}
