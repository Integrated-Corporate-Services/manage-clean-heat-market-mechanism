using Desnz.Chmm.Common.Authorization.Requirements;
using Desnz.Chmm.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Desnz.Chmm.Common.Authorization.Handlers
{
    public class OrganisationAuthorizationHandler : AuthorizationHandler<OrganisationRequirement>
    {
        private readonly ILogger<OrganisationAuthorizationHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public OrganisationAuthorizationHandler(ILogger<OrganisationAuthorizationHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                               OrganisationRequirement requirement)
        {
            var user = context.User;

            if (user.IsAdmin() || user.HasApiRole())
            {
                context.Succeed(requirement);
            }
            else if (user.IsManufacturer())
            {
                HandleRequirementForManufacturer(user, context, requirement);
            }
            else
            {
                _logger.LogInformation("User {userId} does not have a valid role", user.GetUserId());
            }

            return Task.CompletedTask;
        }

        private void HandleRequirementForManufacturer(ClaimsPrincipal user, AuthorizationHandlerContext context, OrganisationRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            var organisationId = httpContext.GetOrganisationId();
            var userOrganisationId = user.GetOrganisationId();

            if (organisationId == null)
            {
                _logger.LogWarning("Could not retrieve an organisationId value from the request");
            }
            else
            {
                if (organisationId == userOrganisationId)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    _logger.LogInformation("Organisation id {organisationId} does not match user's {userId} organisationId claim value {userOrganisationId}",
                        organisationId, user.GetUserId(), userOrganisationId);
                }
            }
        }
    }
}
