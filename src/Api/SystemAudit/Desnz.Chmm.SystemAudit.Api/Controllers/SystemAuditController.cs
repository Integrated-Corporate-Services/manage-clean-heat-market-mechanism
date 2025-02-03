using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.SystemAudit.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.SystemAudit.Api.Controllers;

[ApiController]
[Route("api/systemaudit")]
[Authorize(Roles = Roles.Admins)]
public class SystemAuditController : ControllerBase
{
    private readonly IMediator _mediator;

    public SystemAuditController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns a list of events representing Audit Items of Command AuditType for a particular organisation
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <response code="200">The list of events</response>
    /// <response code="400">Failed to get organisation</response>
    /// <response code="400">"Failed to get manufacturer users for organisation</response>
    /// <response code="400">""Failed to get admin users</response>
    [HttpGet]
    [Route("organisation/{organisationId:guid}")]
    public async Task<ActionResult<List<AuditItemDto>>> GetForOrganisation(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetOrganisationAuditItemsQuery(organisationId), cancellationToken);
    }
}