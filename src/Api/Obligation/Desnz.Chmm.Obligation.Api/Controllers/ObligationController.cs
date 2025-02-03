using Desnz.Chmm.Common.Authorization.Constants;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.Obligation.Common.Dtos;
using Desnz.Chmm.Obligation.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Obligation.Api.Controllers;

/// <summary>
/// Obligation API
/// </summary>
/// 
[ApiController]
[Route("api/obligation")]
[Authorize]
public class ObligationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ObligationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Amends a manufacturer's obligation
    /// </summary>
    /// <param name="command">Object describing the obligation amendment amount for a particular organisation during the specified scheme year</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="201">Returns the ID of the 'Admin Adjustment' transaction that was created</response>
    /// <response code="400">Invalid command or command properties</response>
    /// <response code="400">No organisation with the specified ID exists</response>
    /// <response code="400">Organisation with the specified ID is not 'Active'</response>
    /// <response code="400">User is not allowed to access the organisation with the specified ID</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("amend")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> AmendObligation([FromBody]AmendObligationCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Submit annual boiler obligation
    /// </summary>
    /// <param name="command">Annual boiler obligation data</param>
    /// <response code="201">Successfully submitted annual boiler obligation data</response>
    /// <response code="400">Invalid command or command properties</response>
    /// <response code="400">No organisation with the specified ID exists</response>
    /// <response code="400">Organisation with the specified ID is not 'Active'</response>
    /// <response code="400">User is not allowed to access the organisation with the specified ID</response>
    [HttpPost("annual")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Guid>> SubmitAnnualBoilerObligation([FromBody] CreateAnnualObligationCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Submit quarterly boiler obligation
    /// </summary>
    /// <param name="command">Quarterly boiler obligation data</param>
    /// <response code="200">Successfully submitted quarterly boiler obligation data</response>
    /// <response code="400">Invalid command or command properties</response>
    /// <response code="400">No organisation with the specified ID exists</response>
    /// <response code="400">Organisation with the specified ID is not 'Active'</response>
    /// <response code="400">User is not allowed to access the organisation with the specified ID</response>
    [HttpPost("quarterly")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Guid>> SubmitQuarterlyBoilerObligation([FromBody] CreateQuarterlyObligationCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Returns an obligation summary for the organisation detailing the brought forward, total generated, adjustments and carry forward
    /// </summary>
    /// <param name="organisationId">The organisation to query</param>
    /// <param name="schemeYearId">The scheme year to query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns the 'obligation summary' of an organisation's obligation during the specified scheme year</response>
    [HttpGet]
    [Route("organisation/{organisationId:guid}/year/{schemeYearId:guid}/summary")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<ObligationSummaryDto>> GetObligationSummary([FromRoute] Guid organisationId, [FromRoute] Guid schemeYearId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetObligationSummaryQuery(organisationId, schemeYearId), cancellationToken);
    }

    /// <summary>
    /// Returns the totals for all organisations in the given scheme year
    /// </summary>
    /// <param name="schemeYearId">The scheme year to query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns the 'obligation summary totals' of all organisations obligation during the specified scheme year</response>
    [HttpGet]
    [Route("year/{schemeYearId:guid}/totals")]
    [Authorize(Roles = Roles.ApiRole)]
    public async Task<ActionResult<List<ObligationTotalDto>>> GetSchemeYearObligationTotals([FromRoute] Guid schemeYearId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetSchemeYearObligationTotalsQuery(schemeYearId), cancellationToken);
    }

    [HttpPost("redeem")]
    [Authorize(Roles = Roles.ApiRole)]
    public async Task<ActionResult> LogRedemption([FromBody] RedeemObligationsCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPost("redeem/rollback")]
    [Authorize(Roles = Roles.ApiRole)]
    public async Task<ActionResult> RollbackRedemption([FromBody] RollbackRedeemObligationsCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("carryforward")]
    [Authorize(Roles = Roles.ApiRole)]
    public async Task<ActionResult<List<Guid>>> CarryForwardObligation([FromBody] CarryForwardObligationCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPost("carryforward/rollback")]
    [Authorize(Roles = Roles.ApiRole)]
    public async Task<ActionResult> RollbackCarryForwardObligation([FromBody] RollbackCarryForwardObligationCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}