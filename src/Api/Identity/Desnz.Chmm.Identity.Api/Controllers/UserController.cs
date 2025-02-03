using static Desnz.Chmm.Common.Constants.IdentityConstants;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.Authorization.Constants;

namespace Desnz.Chmm.Identity.Api.Controllers;

/// <summary>
/// Users API
/// </summary>
[ApiController]
[Route("api/identity/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediator;

    public UserController(
        ILogger<UserController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of all users
    /// </summary>
    /// <response code="200">Successfully retrieved list of users</response>
    [HttpGet]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<List<ChmmUserDto>>> GetAll(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetAdminUsersQuery(), cancellationToken);
    }

    /// <summary>
    /// Get list of admin users
    /// </summary>
    /// <response code="200">Successfully retrieved list of admin users</response>
    [HttpGet("admin")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<List<ChmmUserDto>>> GetAdminUsers(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetAdminUsersQuery(), cancellationToken);
    }

    /// <summary>
    /// Get list of manufacturer users
    /// </summary>
    /// <param name="organisationId">Organisation id to get users for</param>
    /// <response code="200">Successfully retrieved list of manufacturer users</response>
    [HttpGet("manufacturer/{organisationId:guid}")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<List<ViewManufacturerUserDto>>> GetManufacturerUsers(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetManufacturerUsersQuery() { OrganisationId = organisationId }, cancellationToken);
    }

    /// <summary>
    /// Get admin user
    /// </summary>
    /// <param name="userId">Admin user id</param>
    /// <response code="200">Successfully retrieved admin user</response>
    /// <response code="404">Could not find admin user</response>
    [HttpGet("admin/{userId:guid}")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<ChmmUserDto>> GetAdminUser(Guid userId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetAdminUserQuery() { UserId = userId }, cancellationToken);
    }

    /// <summary>
    /// Invite admin user
    /// </summary>
    /// <param name="command">Invite admin user command</param>
    /// <response code="204">Successfully invited admin user</response>
    /// <response code="404">Invalid role id</response>
    /// <response code="400">Invalid user details</response>
    [HttpPost("admin")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<Guid>> InviteAdminUser([FromBody] InviteAdminUserCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit existing admin user
    /// </summary>
    /// <param name="command">Edit admin user command</param>
    /// <response code="204">Successfully edited admin user details</response>
    /// <response code="404">Invalid user id</response>
    /// <response code="404">Invalid role id</response>
    /// <response code="400">Invalid user details</response>
    [HttpPut("admin")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditAdminUser([FromBody] EditAdminUserCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit existing admin user
    /// </summary>
    /// <param name="command">Edit admin user command</param>
    /// <response code="204">Successfully edited admin user details</response>
    /// <response code="404">Invalid user id</response>
    /// <response code="404">Invalid role id</response>
    /// <response code="400">Invalid user details</response>
    [HttpPut("manufacturer")]
    [Authorize(Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult> EditManufacturerUser([FromBody] EditManufacturerUserCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Activate admin user
    /// </summary>
    /// <param name="command">Activate admin user command</param>
    /// <response code="204">Successfully activated admin user</response>
    /// <response code="404">Invalid user id</response>
    /// <response code="400">Invalid status</response>
    [HttpPut("admin/activate")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> ActivateAdminUser([FromBody] ActivateAdminUserCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Deactivate admin user
    /// </summary>
    /// <param name="command">Deactivate admin user command</param>
    /// <response code="204">Successfully deactivated admin user</response>
    /// <response code="404">Invalid user id</response>
    /// <response code="400">Invalid status</response>
    [HttpPut("admin/deactivate")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> DeactivateAdminUser([FromBody] DeactivateAdminUserCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Invite manufacturer user
    /// </summary>
    /// <param name="command">Invite manufacturer user command</param>
    /// <response code="204">Successfully invited manufacturer user</response>
    /// <response code="400">Invalid user details</response>
    [HttpPost("manufacturer")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Guid>> InviteManufacturerUser([FromBody] InviteManufacturerUserCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Deactivate manufacturer user
    /// </summary>
    /// <param name="command">Deactivate manufacturer user command</param>
    /// <response code="204">Successfully deactivate manufacturer user</response>
    /// <response code="400">Invalid user details</response>
    [HttpPost("manufacturer/deactivate")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Guid>> DeactivateManufacturerUser([FromBody] DeactivateManufacturerUserCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Get manufacturer user
    /// </summary>
    /// <param name="organisationId">Manufacturer organisation id</param>
    /// <param name="userId">Manufacturer user id</param>
    /// <response code="200">Successfully retrieved manufacturer user</response>
    /// <response code="404">Could not find manufacturer organisation</response>
    /// <response code="404">Could not find manufacturer user</response>
    [HttpGet("manufacturer/{organisationId:guid}/users/{userId:guid}")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<ViewManufacturerUserDto>> GetManufacturerUser(Guid organisationId, Guid userId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetManufacturerUserQuery(organisationId, userId), cancellationToken);
    }
}
