using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
using Desnz.Chmm.Common.Authorization.Constants;

namespace Desnz.Chmm.BoilerSales.Api.Controllers;

/// <summary>
/// Boiler sales API
/// </summary>
[ApiController]
[Route("api/boilersales")]
[Authorize(Roles = Roles.Admins)]
public class AdminFunctionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminFunctionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Annual boiler sales

    /// <summary>
    /// Approve the given annual boiler sales for an organistaion
    /// </summary>
    /// <param name="command">The details of the boiler sales to approve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Successfully approved the sales figures</response>
    /// <response code="400">Failed to get organisation</response>
    /// <response code="400">Organisation is not active</response>
    /// <response code="400">Invalid boiler sales status for approval</response>
    /// <response code="404">Annual boiler sales not found for scheme year</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual/approve")]
    public async Task<ActionResult> ApproveAnnualBoilerSalesForManufacturer([FromBody] ApproveAnnualBoilerSalesCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit the annual boiler sales for an organistaion
    /// </summary>
    /// <param name="command">The details of the boiler sales to edit</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Successfully edited the sales figures</response>
    /// <response code="400">Failed to get organisation</response>
    /// <response code="400">Organisation is not active</response>
    /// <response code="400">Invalid boiler sales status for editing</response>
    /// <response code="404">Annual boiler sales not found for scheme year</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual/edit")]
    public async Task<ActionResult> EditAnnualBoilerSalesForManufacturer([FromBody] EditAnnualBoilerSalesCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Copy annual boiler sales files for editing
    /// </summary>
    /// <param name="command">The details of the boiler sales to edit</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Successfully copied the files</response>
    /// <response code="400">Failed to get organisation</response>
    /// <response code="400">Organisation is not active</response>
    /// <response code="400">Invalid boiler sales status for editing</response>
    /// <response code="404">Annual boiler sales not found for scheme year</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual/edit/copy-files")]
    public async Task<ActionResult> EditAnnualBoilerSalesForManufacturerCopyFiles([FromBody] EditAnnualBoilerSalesCopyFilesCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    #endregion

    #region Quarterly boiler sales

    /// <summary>
    /// Update quarterly boiler sales
    /// </summary>
    /// <param name="command">Quarterly boiler sales data</param>
    /// <param name="cancellationToken"></param>
    /// <response code="204">Successfully updated quarterly boiler sales data</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/quarter/{schemeYearQuarterId:guid}/edit")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.Admins)]
    public async Task<ActionResult> EditQuarterlyBoilerSalesForManufacturer([FromBody] EditQuarterlyBoilerSalesCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Copy annual boiler sales files for editing
    /// </summary>
    /// <param name="command">The details of the boiler sales to edit</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Successfully copied the files</response>
    /// <response code="400">Failed to get organisation</response>
    /// <response code="400">Organisation is not active</response>
    /// <response code="400">Invalid boiler sales status for editing</response>
    /// <response code="404">Annual boiler sales not found for scheme year</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/quarter/{schemeYearQuarterId:guid}/edit/copy-files")]
    public async Task<ActionResult> EditQuarterlyBoilerSalesForManufacturerCopyFiles([FromBody] EditQuarterlyBoilerSalesCopyFilesCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    #endregion
}
