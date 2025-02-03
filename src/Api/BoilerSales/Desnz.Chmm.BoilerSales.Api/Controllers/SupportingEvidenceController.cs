using Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly.SupportingEvidence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using Desnz.Chmm.BoilerSales.Common.Dtos.Annual;
using Desnz.Chmm.BoilerSales.Common.Dtos.Quarterly;
using Desnz.Chmm.Common.Authorization.Constants;

namespace Desnz.Chmm.BoilerSales.Api.Controllers;

/// <summary>
/// Boiler sales API
/// </summary>
[ApiController]
[Route("api/boilersales")]
[Authorize]
public class SupportingEvidenceController : ControllerBase
{
    private readonly IMediator _mediator;

    public SupportingEvidenceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get annual supporting evidence file names
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <response code="200">Successfully retrieved annual supporting evidence file names</response>
    [HttpGet("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual/supporting-evidence")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<List<string>>> GetAnnualSupportingEvidenceFileNames(Guid organisationId, Guid schemeYearId, [FromQuery] bool? isEditing, CancellationToken cancellationToken)
    {
        var query = new GetAnnualSupportingEvidenceFileNamesQuery(
            organisationId,
            schemeYearId,
            isEditing
        );
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Upload annual supporting evidence
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <param name="supportingEvidence">Supporting evidence</param>
    /// <response code="200">Successfully uploaded annual supporting evidence</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual/supporting-evidence")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult> UploadAnnualSupportingEvidence(Guid organisationId, Guid schemeYearId, [FromForm] List<IFormFile> supportingEvidence, [FromQuery] bool? isEditing, CancellationToken cancellationToken)
    {
        var command = new UploadAnnualSupportingEvidenceCommand(
            organisationId,
            schemeYearId,
            supportingEvidence,
            isEditing
        );
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Delete annual supporting evidence
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <param name="body">Supporting evidence to delete</param>
    /// <response code="200">Successfully deleted supporting evidence</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual/supporting-evidence/delete")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult> DeleteAnnualSupportingEvidence(Guid organisationId, Guid schemeYearId, [FromBody] DeleteAnnualBoilerSalesFileDto body, [FromQuery] bool? isEditing, CancellationToken cancellationToken)
    {
        var command = new DeleteAnnualSupportingEvidenceCommand(
            organisationId,
            schemeYearId,
            body.FileName,
            isEditing
        );
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Download annual supporting evidence
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <param name="fileName">Supporting evidence file to download</param>
    /// <response code="200">Successfully downloaded supporting evidence</response>
    [HttpGet("organisation/{organisationId:guid}/year/{schemeYearId:guid}/annual/supporting-evidence/download")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Stream>> DownloadAnnualSupportingEvidence([FromRoute] Guid organisationId, [FromRoute] Guid schemeYearId, [FromQuery] string fileName, [FromQuery] bool? isEditing, CancellationToken cancellationToken)
    {
        var query = new DownloadAnnualSupportingEvidenceQuery(
            organisationId,
            schemeYearId,
            fileName,
            isEditing
        );

        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Get quarterly supporting evidence file names
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <param name="schemeYearQuarterId">Id of scheme year quarter to get data for</param>
    /// <param name="isEditing">Is editing</param>
    /// <response code="200">Successfully </response>
    [HttpGet("organisation/{organisationId:guid}/year/{schemeYearId:guid}/quarter/{schemeYearQuarterId:guid}/supporting-evidence")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<List<string>>> GetQuarterlySupportingEvidenceFileNames(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId, bool isEditing, CancellationToken cancellationToken)
    {
        var query = new GetQuarterlySupportingEvidenceFileNamesQuery(organisationId, schemeYearId, schemeYearQuarterId, isEditing);
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Upload quarterly supporting evidence
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <param name="schemeYearQuarterId">Id of scheme year quarter to get data for</param>
    /// <param name="supportingEvidence">Supporting evidence files</param>
    /// <param name="isEditing">Is editing</param>
    /// <response code="200">Successfully uploaded supporting evidence</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/quarter/{schemeYearQuarterId:guid}/supporting-evidence")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Guid>> UploadQuarterlySupportingEvidence(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId, bool isEditing, [FromForm] List<IFormFile> supportingEvidence, CancellationToken cancellationToken)
    {
        var command = new UploadQuarterlySupportingEvidenceCommand(
            organisationId,
            schemeYearId,
            schemeYearQuarterId,
            supportingEvidence,
            isEditing
        );
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Delete quarterly supporting evidence
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <param name="schemeYearQuarterId">Id of scheme year quarter to get data for</param>
    /// <param name="body">Supporting evidence files to delete</param>
    /// <param name="isEditing">Is editing</param>
    /// <response code="200">Successfully deleted supporting evidence files</response>
    [HttpPost("organisation/{organisationId:guid}/year/{schemeYearId:guid}/quarter/{schemeYearQuarterId:guid}/supporting-evidence/delete")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Guid>> DeleteQuarterlySupportingEvidence(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId, bool isEditing, [FromBody] DeleteQuarterlyBoilerSalesFileDto body, CancellationToken cancellationToken)
    {
        var command = new DeleteQuarterlySupportingEvidenceCommand(
            organisationId,
            schemeYearId,
            schemeYearQuarterId,
            body.FileName,
            isEditing
        );
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Download quarterly supporting evidence
    /// </summary>
    /// <param name="organisationId">Id of organisation to get data for</param>
    /// <param name="schemeYearId">Id of scheme year to get data for</param>
    /// <param name="schemeYearQuarterId">Id of scheme year quarter to get data for</param>
    /// <param name="fileName">Supporting evidence file name to download</param>
    /// <response code="200">Successfully downloaded supporting evidence file</response>
    [HttpGet("organisation/{organisationId:guid}/year/{schemeYearId:guid}/quarter/{schemeYearQuarterId:guid}/supporting-evidence/download")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Stream>> DownloadQuarterlySupportingEvidence([FromRoute] Guid organisationId, [FromRoute] Guid schemeYearId, Guid schemeYearQuarterId, [FromQuery] string fileName, CancellationToken cancellationToken)
    {
        var query = new DownloadQuarterlySupportingEvidenceQuery(
            organisationId,
            schemeYearId,
            schemeYearQuarterId,
            fileName
        );

        return await _mediator.Send(query, cancellationToken);
    }
}
