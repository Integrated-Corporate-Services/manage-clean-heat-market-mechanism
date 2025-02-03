using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Identity.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Identity.Api.Controllers;

/// <summary>
/// Roles API
/// </summary>
[ApiController]
[Route("api/identity/roles")]
[Authorize(Roles = Roles.Admins)]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of admin roles
    /// </summary>
    /// <response code="200">Successfully retrieved roles</response>
    [HttpGet("admin")]
    public async Task<ActionResult<List<RoleDto>>>GetAdminRoles(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetAdminRolesQuery(), cancellationToken);
    }
}
