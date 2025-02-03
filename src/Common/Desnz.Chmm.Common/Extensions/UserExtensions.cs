using Desnz.Chmm.Common.Mediator;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using System.IdentityModel.Tokens.Jwt;

namespace Desnz.Chmm.Common.Extensions;

public static class UserExtensions
{
    public static Guid? GetOrganisationId(this ClaimsPrincipal user)
    {
        var sOrganisationId = user.Claims.FirstOrDefault(c => c.Type == Claims.OrganisationId)?.Value; ;
        return Guid.TryParse(sOrganisationId, out var organisationId) ? organisationId : null;
    }


    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var sUserId = user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid)?.Value;
        return Guid.TryParse(sUserId, out var userId) ? userId : null;
    }

    public static string GetEmail(this ClaimsPrincipal user) => user.Claims.Single(c => c.Type == ClaimTypes.Email).Value;

    public static (bool, ActionResult?) CanAccessOrganisation(this ClaimsPrincipal user, Guid? organisationId)
    {
        // If admin, return true
        if (user.IsAdmin())
            return (true, null);

        // Otherwise if manufacturer
        else if (user.IsInRole(Roles.Manufacturer))
        {
            var organisationClaim = user.GetOrganisationId();
            if (organisationClaim == null)
                return (false, Responses.BadRequest("User does not have an organisation claim"));
            else if (organisationId == null || organisationClaim != organisationId)
                return (false, Responses.BadRequest("User does not have access to this organisation"));
            return (true, null);
        }

        // Otherwise return false
        else return (false, Responses.BadRequest("User is not an admin or a manufacturer"));
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole(Roles.RegulatoryOfficer)
            || user.IsInRole(Roles.SeniorTechnicalOfficer)
            || user.IsInRole(Roles.PrincipalTechnicalOfficer);
    }

    public static bool IsManufacturer(this ClaimsPrincipal user) => user.IsInRole(Roles.Manufacturer);
    public static bool HasApiRole(this ClaimsPrincipal user) => user.IsInRole(Roles.ApiRole);


}
