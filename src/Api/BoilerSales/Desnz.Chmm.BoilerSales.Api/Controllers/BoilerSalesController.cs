using Desnz.Chmm.BoilerSales.Common.Commands.Annual;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using Desnz.Chmm.BoilerSales.Common.Dtos.Annual;
using Desnz.Chmm.BoilerSales.Common.Dtos.Quarterly;
using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.Common.Authorization.Constants;

namespace Desnz.Chmm.BoilerSales.Api.Controllers;

/// <summary>
/// Boiler sales API
/// </summary>
[ApiController]
[Route("api/boilersales")]
[Authorize]
public class BoilerSalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoilerSalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get annual boiler sales for your own organisation
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="schemeYearId">Id of scheme year to get data for (currently not in use)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Successfully retrieved annual boiler sales</response>
    [HttpGet("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<AnnualBoilerSalesDto>> GetAnnualBoilerSales(Guid organisationId, Guid schemeYearId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetAnnualBoilerSalesQuery(organisationId, schemeYearId), cancellationToken);
    }

    /// <summary>
    /// Get quarterly boiler sales for your own organisation
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="schemeYearId">Id of scheme year to get data for (currently not in use)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Successfully retrieved quarterly boiler sales</response>
    [HttpGet("organisation/{organisationId:guid}/year/{schemeYearId:guid}/quarters")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<List<QuarterlyBoilerSalesDto>>> GetQuarterlyBoilerSales(Guid organisationId, Guid schemeYearId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetQuarterlyBoilerSalesQuery(organisationId, schemeYearId), cancellationToken);
    }

    /// <summary>
    /// Submit annual boiler sales
    /// </summary>
    /// <param name="command">Annual boiler sales data</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Successfully submitted annual boiler sales data</response>
    /// <response code="400">Failed to get organisation</response>
    /// <response code="400">Organisation is not active</response>
    /// <response code="400">Annual boiler sales already exists for scheme year</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Guid>> SubmitAnnualBoilerSales([FromBody] SubmitAnnualBoilerSalesCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Submit quarterly boiler sales
    /// </summary>
    /// <param name="command">Quarterly boiler sales data</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Successfully submitted quarterly boiler sales data</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/quarter/{schemeYearQuarterId:guid}")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Guid>> SubmitQuarterlyBoilerSales([FromBody] SubmitQuarterlyBoilerSalesCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Return the boiler sales summary for the given organisation and scheme year
    /// </summary>
    /// <param name="organisationId">The organisation to query</param>
    /// <param name="schemeYearId">The scheme year to query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A summary of the organisation's boiler sales</returns>
    [HttpGet]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    [Route("organisation/{organisationId:guid}/year/{schemeYearId:guid}/summary")]
    public async Task<ActionResult<BoilerSalesSummaryDto>> BoilerSalesSummary(Guid organisationId, Guid schemeYearId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new BoilerSalesSummaryQuery(organisationId, schemeYearId), cancellationToken);
    }

    /// <summary>
    /// Return the boiler sales summary for the given organisation and scheme year
    /// </summary>
    /// <param name="organisationId">The organisation to query</param>
    /// <param name="schemeYearId">The scheme year to query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A summary of the organisation's boiler sales</returns>
    [HttpGet]
    [Authorize(Roles = Roles.AdminsAndApi)]
    [Route("year/{schemeYearId:guid}/summary")]
    public async Task<ActionResult<List<BoilerSalesSummaryDto>>> BoilerSalesAllSummary(Guid schemeYearId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new BoilerSalesSummaryAllQuery(schemeYearId), cancellationToken);
    }
}
