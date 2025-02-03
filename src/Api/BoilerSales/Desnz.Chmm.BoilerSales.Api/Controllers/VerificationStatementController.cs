
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.VerificationStatement;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual.VerificationStatement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using Desnz.Chmm.BoilerSales.Common.Dtos.Annual;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.Authorization.Constants;

namespace Desnz.Chmm.BoilerSales.Api.Controllers;

/// <summary>
/// Boiler sales API
/// </summary>
[ApiController]
[Route("api/boilersales")]
[Authorize]
public class VerificationStatementController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _userService;

    public VerificationStatementController(IMediator mediator, ICurrentUserService userService)
    {
        _mediator = mediator;
        _userService = userService;
    }

    /// <summary>
    /// Get annual verification statement file names
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <response code="200">Successfully retrieved annual verification statement file names</response>
    [HttpGet("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual/verification-statement")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<List<string>>> GetAnnualVerificationStatementFileNames(Guid organisationId, Guid schemeYearId, [FromQuery] bool? isEditing, CancellationToken cancellationToken)
    {
        var query = new GetAnnualVerificationStatementFileNamesQuery(
            organisationId,
            schemeYearId,
            isEditing
        );
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Upload annual verification statement
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <param name="verificationStatement">Verification statements</param>
    /// <response code="200">Successfully uploaded verification statements</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual/verification-statement")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult> UploadAnnualVerificationStatement(Guid organisationId, Guid schemeYearId, [FromForm] List<IFormFile> verificationStatement, [FromQuery] bool? isEditing, CancellationToken cancellationToken)
    {
        var command = new UploadAnnualVerificationStatementCommand(
            organisationId,
            schemeYearId,
            verificationStatement,
            isEditing
        );
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Delete annual verification statement
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <param name="body">Verification statement to delete</param>
    /// <response code="200">Successfully deleted verification statement</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual/verification-statement/delete")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult> DeleteAnnualVerificationStatement(Guid organisationId, Guid schemeYearId, [FromBody] DeleteAnnualBoilerSalesFileDto body, [FromQuery] bool? isEditing, CancellationToken cancellationToken)
    {
        var command = new DeleteAnnualVerificationStatementCommand(
            organisationId,
            schemeYearId,
            body.FileName,
            isEditing
        );
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Download annual verification statement
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <param name="fileName">Verification statement file to download</param>
    /// <response code="200">Successfully downloaded verification statement</response>
    [HttpGet("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual/verification-statement/download")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Stream>> DownloadAnnualVerificationStatement([FromRoute] Guid organisationId, [FromRoute] Guid schemeYearId, [FromQuery] string fileName, [FromQuery] bool? isEditing, CancellationToken cancellationToken)
    {
        var query = new DownloadAnnualVerificationStatementQuery(
            organisationId,
            schemeYearId,
            fileName,
            isEditing
        );

        return await _mediator.Send(query, cancellationToken);
    }
}
