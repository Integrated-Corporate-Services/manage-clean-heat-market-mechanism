using Desnz.Chmm.Common.Authorization.Constants;
using Desnz.Chmm.Identity.Api.Handlers.Commands;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.Identity.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Identity.Api.Controllers;

/// <summary>
/// Organisation API
/// </summary>
[ApiController]
[Produces("application/json")]
[Route("api/identity/licenceholders")]
[Authorize]
public class LicenceHolderController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// DI Constructor
    /// </summary>
    /// <param name="mediator"></param>
    public LicenceHolderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a licence holder
    /// </summary>
    /// <param name="command">The licence holder to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Successfully created the licence holder</response>
    /// <response code="400">Organisation name changes</response>
    [HttpPost]

    [Authorize(Roles = Roles.AdminsAndApi)]
    public async Task<ActionResult<Guid>> CreateLicenceHolder([FromBody] CreateLicenceHolderCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Create a licence holder
    /// </summary>
    /// <param name="command">The licence holder to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Successfully created the licence holders</response>
    [HttpPost("batch")]
    [Authorize(Roles = Roles.AdminsAndApi)]
    public async Task<ActionResult<List<Guid>>> CreateLicenceHolders([FromBody] CreateLicenceHoldersCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Get list of all licence holders
    /// </summary>
    /// <response code="200">Successfully retrieved licence holders</response>
    [HttpGet("all")]
    [Authorize(Roles = Roles.AdminsAndApi)]
    public async Task<ActionResult<List<LicenceHolderDto>>> GetAll(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetLicenceHoldersAllQuery(), cancellationToken);
    }

    /// <summary>
    /// Get list of all licence holders
    /// </summary>
    /// <response code="200">Successfully retrieved licence holders</response>
    [HttpGet("links/all")]
    [Authorize(Roles = Roles.AdminsAndApi)]
    public async Task<ActionResult<List<LicenceHolderLinkDto>>> GetAllLinks(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetLicenceHoldersAllLinksQuery(), cancellationToken);
    }

    /// <summary>
    /// Get list of all licence holders linked to a specific organisation
    /// </summary>
    /// <param name="organisationId">Organisation id to get licence holders for</param>
    /// <response code="200">Successfully retrieved licence holders</response>
    [HttpGet("linked-to/{organisationId:guid}")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.Everyone)]
    public async Task<ActionResult<List<LicenceHolderLinkDto>>> GetLinkedTo(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetLicenceHolderLinksQuery(organisationId), cancellationToken);
    }

    /// <summary>
    /// Get list of all licence holders linked to a specific organisation
    /// </summary>
    /// <param name="licenceHolderId">Organisation id to get licence holders for</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Successfully retrieved licence holders</response>
    [HttpGet("exists/{licenceHolderId:guid}")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.Everyone)]
    public async Task<ActionResult<LicenceHolderExistsDto>> LicenceHolderExists(Guid licenceHolderId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new LicenceHolderExistsQuery(licenceHolderId), cancellationToken);
    }

    /// <summary>
    /// Get list of all licence holders linked to a specific organisation
    /// </summary>
    /// <param name="organisationId">Organisation id to get licence holders for</param>
    /// <response code="200">Successfully retrieved licence holders</response>
    [HttpGet("linked-to/{organisationId:guid}/all")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.Everyone)]
    public async Task<ActionResult<List<LicenceHolderLinkDto>>> GetLicenceHolderLinksHistory(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetLicenceHolderLinksHistoryQuery(organisationId), cancellationToken);
    }

    /// <summary>
    /// Get list of all licence holders that are unlinked to any organisation
    /// </summary>
    /// <response code="200">Successfully retrieved licence holders</response>
    [HttpGet("unlinked")]
    [Authorize(Roles = Roles.AdminsAndApi)]
    public async Task<ActionResult<List<LicenceHolderDto>>> GetUnlinked(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetLicenceHoldersUnlinkedQuery(), cancellationToken);
    }

    /// <summary>
    /// Link licence holder to organisation
    /// </summary>
    /// <param name="licenceHolderId">Licence holder id to link to organisation</param>
    /// <param name="organisationId">Organisation id</param>
    /// <param name="startDate">Link start date</param>n
    /// <response code="204">Successfully linked licence holder to organisation</response>
    /// <response code="404">Licence holder not found</response>
    /// <response code="404">Organisation not found</response>
    /// <response code="400">Licence holder already linked</response>
    [HttpPost("{licenceHolderId:guid}/link-to/{organisationId:guid}")]
    [Authorize(Roles = Roles.AdminsAndApi)]
    public async Task<ActionResult> Link(Guid licenceHolderId, Guid organisationId, [FromBody] LinkLicenceHolderDto? linkLicenceHolderDto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new LinkLicenceHolderCommand(licenceHolderId, organisationId, linkLicenceHolderDto.StartDate), cancellationToken);
    }

    /// <summary>
    /// Edit licence holder
    /// </summary>
    /// <param name="licenceHolderId">Licence holder id edit</param>
    /// <param name="endDate">Organisation id for new licence holder link</param>
    /// <param name="organisationId">Date from which the link between the licence holder and organisation ends</param>
    /// <response code="204">Successfully edited the license holder and created new link if organisation changed</response>
    /// <response code="404">Licence holder not found</response>
    /// <response code="400">Licence holder already unlinked</response>
    [HttpPost("{licenceHolderId:guid}/endlink/{organisationId:guid}")]
    [Authorize(Roles = Roles.AdminsAndApi)]
    public async Task<ActionResult> Endlink(Guid licenceHolderId, Guid organisationId, [FromBody] EndLinkLicenceHolderDto endLinkLicenceHolderDto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new EndLinkLicenceHolderCommand(licenceHolderId, organisationId, endLinkLicenceHolderDto.EndDate, endLinkLicenceHolderDto.OrganisationIdToTransfer), cancellationToken);
    }
}
