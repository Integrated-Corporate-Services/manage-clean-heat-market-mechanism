using Desnz.Chmm.Common.Authorization.Constants;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Commands.ApprovalFiles;
using Desnz.Chmm.Identity.Common.Commands.RejectionFiles;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using Desnz.Chmm.Identity.Common.Commands.StructureFiles;

namespace Desnz.Chmm.Identity.Api.Controllers;

/// <summary>
/// Organisation API
/// </summary>
[ApiController]
[Produces("application/json")]
[Route("api/identity/organisations")]
[Authorize]
public class OrganisationController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganisationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all manufacturer organisations
    /// </summary>
    /// <response code="200">Successfully retrieved manufacturer organisations</response>
    [HttpGet("{organisationId:guid}/available-for-transfer")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<List<ViewOrganisationDto>>> GetOrganisationsAvailableForTransfer(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetOrganisationsAvailableForTransferQuery(organisationId), cancellationToken);
    }

    /// <summary>
    /// Get all manufacturer organisations
    /// </summary>
    /// <response code="200">Successfully retrieved manufacturer organisations</response>
    [HttpGet]
    [Authorize(Roles = Roles.AdminsAndApi)]
    public async Task<ActionResult<List<ViewOrganisationDto>>> GetManufacturers(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetOrganisationsQuery(), cancellationToken);
    }

    /// <summary>
    /// Lookup the name of the organisations who's Ids are provided
    /// </summary>
    /// <param name="query">The list of Ids to return the names of</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("name-lookup")]
    [Authorize(Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<List<OrganisationNameDto>>> LookupOrganisationNames([FromBody] OrganisationNameLookupQuery query, CancellationToken cancellationToken)
    {
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Returns a list of all organisations that provided contact details during signup.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{organisationId:guid}/contactable")]
    [Authorize(Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<List<OrganisationContactDetailsDto>>> GetContactableManufacturers(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetContactableManufacturersQuery(organisationId), cancellationToken);
    }

    /// <summary>
    /// Returns a count of all organisations that provided contact details during signup.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{organisationId:guid}/contactable/count")]
    [Authorize(Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<int>> GetContactableManufacturersCount(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetContactableManufacturersCountQuery(organisationId), cancellationToken);
    }

    /// <summary>
    /// Get all manufacturer organisations
    /// </summary>
    /// <response code="200">Successfully retrieved manufacturer organisations</response>
    [HttpGet("active")]
    [Authorize(Roles = Roles.AdminsAndApi)]
    public async Task<ActionResult<List<ViewOrganisationDto>>> GetActiveManufacturers(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetActiveOrganisationsQuery(), cancellationToken);
    }

    /// <summary>
    /// Get manufacturer organisation by id
    /// </summary>
    /// <param name="organisationId">Id of manufacturer organisation to retrieve</param>
    /// <returns>Manufacturer organisation, if exists</returns>
    /// <response code="200">Successfully retrieved manufacturer organisation</response>
    /// <response code="404">Manufacturer does not exist</response>
    [HttpGet("{organisationId:guid}")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<GetEditableOrganisationDto>> GetManufacturer(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetOrganisationQuery { OrganisationId = organisationId }, cancellationToken);
    }

    /// <summary>
    /// Create manufacturer organisation (onboard)
    /// </summary>
    /// <param name="command">Onboard manufacturer command</param>
    /// <response code="201">Successfully onboarded manufacturer</response>
    /// <response code="400">Invalid data provided</response>
    [HttpPost("onboard")]
    public async Task<ActionResult<Guid>> OnboardManufacturer([FromForm] OnboardManufacturerCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    #region Approval files

    /// <summary>
    /// Retrieve approval file(s)
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{organisationId:guid}/approval-files")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<List<string>>> GetApprovalFiles(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetApprovalFilesQuery(organisationId), cancellationToken);
    }

    /// <summary>
    /// Upload approval file(s)
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="files"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{organisationId:guid}/approval-files/upload")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> UploadApprovalFiles([FromRoute] Guid organisationId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken)
    {
        var command = new UploadApprovalFilesCommand(organisationId, files);
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Delete approval file
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="fileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{organisationId:guid}/approval-files/delete")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> DeleteApprovalFile([FromRoute] Guid organisationId, [FromForm] string fileName, CancellationToken cancellationToken)
    {
        var command = new DeleteApprovalFilesCommand(organisationId, fileName);
        return await _mediator.Send(command, cancellationToken);
    }

    #endregion

    #region Rejection files

    /// <summary>
    /// Retrieve rejection file(s)
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{organisationId:guid}/rejection-files")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<List<string>>> GetRejectionFiles(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetRejectionFilesQuery(organisationId), cancellationToken);
    }

    /// <summary>
    /// Upload rejection file(s)
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="files"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{organisationId:guid}/rejection-files/upload")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> UploadRejectionFiles([FromRoute] Guid organisationId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken)
    {
        var command = new UploadRejectionFilesCommand(organisationId, files);
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Delete rejection file
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="fileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{organisationId:guid}/rejection-files/delete")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> DeleteRejectionFile([FromRoute] Guid organisationId, [FromForm] string fileName, CancellationToken cancellationToken)
    {
        var command = new DeleteRejectionFilesCommand(organisationId, fileName);
        return await _mediator.Send(command, cancellationToken);
    }

    #endregion

    #region Rejection files

    /// <summary>
    /// Retrieve structure file(s)
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{organisationId:guid}/structure-files")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<List<string>>> GetStructureFiles(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetStructureFilesQuery(organisationId), cancellationToken);
    }

    /// <summary>
    /// Upload structure file(s)
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="files"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{organisationId:guid}/structure-files/upload")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> UploadStructureFiles([FromRoute] Guid organisationId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken)
    {
        var command = new UploadStructureFilesCommand(organisationId, files);
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Delete structure file
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="fileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{organisationId:guid}/structure-files/delete")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> DeleteStructureFile([FromRoute] Guid organisationId, [FromForm] string fileName, CancellationToken cancellationToken)
    {
        var command = new DeleteStructureFilesCommand(organisationId, fileName);
        return await _mediator.Send(command, cancellationToken);
    }

    #endregion
    /// <summary>
    /// Approve organisation
    /// </summary>
    /// <param name="command">Approve organisation command</param>
    /// <response code="204">Successfully approved organisation</response>
    /// <response code="404">Invalid organisation id</response>
    [HttpPut("approve")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> ApproveOrganisation([FromForm] ApproveManufacturerApplicationCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Reject organisation
    /// </summary>
    /// <param name="command">Reject organisation command</param>
    /// <response code="204">Successfully Rejected organisation</response>
    /// <response code="404">Invalid organisation id</response>
    [HttpPut("reject")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> RejectOrganisation([FromForm] RejectManufacturerApplicationCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit existing organisation
    /// </summary>
    /// <param name="command">Edit organisation command</param>
    /// <response code="204">Successfully edited organisation details</response>
    /// <response code="404">Invalid organisation id</response>
    /// <response code="400">Invalid data provided</response>
    [HttpPut("edit")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditOrganisation([FromForm] EditManufacturerCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit the organisation's applicant
    /// </summary>
    /// <param name="command">The updated applicant details</param>
    /// <returns>200</returns>
    /// <response code="200">Successfully updated manufacturer</response>
    /// <response code="404">Manufacturer does not exist</response>
    [HttpPut("edit/applicant")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditOrganisationApplicant([FromForm] EditOrganisationApplicantCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit the organisation's credit contact details
    /// </summary>
    /// <param name="command">The updated credit contact details</param>
    /// <returns>200</returns>
    /// <response code="200">Successfully updated manufacturer</response>
    /// <response code="404">Manufacturer does not exist</response>
    [HttpPut("edit/credit-contact-details")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditOrganisationCreditContactDetails([FromForm] EditOrganisationCreditContactDetailsCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit the organisation's details
    /// </summary>
    /// <param name="command">The updated details</param>
    /// <returns>200</returns>
    /// <response code="200">Successfully updated manufacturer</response>
    /// <response code="404">Manufacturer does not exist</response>
    [HttpPut("edit/details")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditOrganisationDetails([FromForm] EditOrganisationDetailsCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit the organisation's fossil fuel seller flag
    /// </summary>
    /// <param name="command">The updated fossil fuel seller flag</param>
    /// <returns>200</returns>
    /// <response code="200">Successfully updated manufacturer</response>
    /// <response code="404">Manufacturer does not exist</response>
    [HttpPut("edit/fossil-fuel-seller")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditOrganisationFossilFuelSeller([FromForm] EditOrganisationFossilFuelSellerCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit the organisation's heat pump details
    /// </summary>
    /// <param name="command">The updated heat pump details</param>
    /// <returns>200</returns>
    /// <response code="200">Successfully updated manufacturer</response>
    /// <response code="404">Manufacturer does not exist</response>
    [HttpPut("edit/heatpump-seller")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditOrganisationHeatPumpSeller([FromForm] EditOrganisationHeatPumpSellerCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit the organisation's correspondence address
    /// </summary>
    /// <param name="command">The updated correspondence address details</param>
    /// <returns>200</returns>
    /// <response code="200">Successfully updated manufacturer</response>
    /// <response code="404">Manufacturer does not exist</response>
    [HttpPut("edit/legal-correspondence-address")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditOrganisationLegalCorrespondenceAddress([FromBody] EditOrganisationLegalCorrespondenceAddressCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit the organisation's registered office address
    /// </summary>
    /// <param name="command">The updated applicant details</param>
    /// <returns>200</returns>
    /// <response code="200">Successfully updated manufacturer</response>
    /// <response code="404">Manufacturer does not exist</response>
    [HttpPut("edit/registered-office-address")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditOrganisationRegisteredOfficeAddress([FromForm] EditOrganisationRegisteredOfficeAddressCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit the organisation's senior responsible officer
    /// </summary>
    /// <param name="command">The updated senior responsible officer details</param>
    /// <returns>200</returns>
    /// <response code="200">Successfully updated manufacturer</response>
    /// <response code="404">Manufacturer does not exist</response>
    [HttpPut("edit/senior-responsible-officer")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditOrganisationSeniorResponsibleOfficer([FromForm] EditOrganisationSeniorResponsibleOfficerCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit the organisation's structure
    /// </summary>
    /// <param name="command">The updated structure details</param>
    /// <returns>200</returns>
    /// <response code="200">Successfully updated manufacturer</response>
    /// <response code="404">Manufacturer does not exist</response>
    [HttpPut("edit/structure")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditOrganisationStructure([FromForm] EditOrganisationStructureCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit the organisation's scheme participation flag
    /// </summary>
    /// <param name="command">The updated flag details</param>
    /// <returns>200</returns>
    /// <response code="200">Successfully updated manufacturer</response>
    /// <response code="404">Manufacturer does not exist</response>
    [HttpPut("edit/scheme-participation")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditOrganisationSchemeParticipation([FromBody] EditOrganisationSchemeParticipationCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Approve organisation
    /// </summary>
    /// <param name="command">Approve organisation command</param>
    /// <response code="204">Successfully approved organisation</response>
    /// <response code="404">Invalid organisation id</response>
    [HttpGet("{organisationId:guid}/status")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<OrganisationStatusDto>> GetStatus(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetOrganisationStatusQuery(organisationId), cancellationToken);
    }

    [HttpGet("{organisationId:guid}/approval-comments/download")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<Stream>> DownloadApprovalCommentsFile([FromRoute] Guid organisationId, [FromQuery] string fileName, CancellationToken cancellationToken)
    {
        var query = new DownloadApprovalCommentsFileQuery(organisationId, fileName);
        return await _mediator.Send(query, cancellationToken);
    }

    [HttpGet("{organisationId:guid}/organisation-structure/download")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<Stream>> DownloadOrganisationStructureFile([FromRoute] Guid organisationId, [FromQuery] string fileName, CancellationToken cancellationToken)
    {
        var query = new DownloadOrganisationStructureFileQuery(organisationId, fileName);
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Approve organisation
    /// </summary>
    /// <param name="command">Approve organisation command</param>
    /// <response code="204">Successfully approved organisation</response>
    /// <response code="404">Invalid organisation id</response>
    [HttpGet("{organisationId:guid}/approval-comments")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<OrganisationApprovalCommentsDto>> GetApprovalComments(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetApprovalCommentsQuery(organisationId), cancellationToken);
    }

    /// <summary>
    /// Reject organisation
    /// </summary>
    /// <param name="command">Reject organisation command</param>
    /// <response code="204">Successfully rejected organisation</response>
    /// <response code="404">Invalid organisation id</response>
    [HttpGet("{organisationId:guid}/rejection-comments")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<OrganisationRejectionCommentsDto>> GetRejectionComments(Guid organisationId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetRejectionCommentsQuery(organisationId), cancellationToken);
    }


    [HttpGet("{organisationId:guid}/rejection-comments/download")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<Stream>> DownloadRejectionCommentsFile([FromRoute] Guid organisationId, [FromQuery] string fileName, CancellationToken cancellationToken)
    {
        var query = new DownloadRejectionCommentsFileQuery(organisationId, fileName);
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Edit the user assigned to the organisation as senior responsible officer
    /// </summary>
    /// <param name="command">The user id that you want to assign as the Senior Responsible Officer for the organisation</param>
    /// <returns>200</returns>
    /// <response code="200">Successfully updated organisation</response>
    /// <response code="404">Organisation does not exist</response>
    [HttpPut("edit/senior-responsible-officer-assigned")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> EditUserAssignedAsSeniorResponsibleOfficer(EditOrganisationSeniorResponsibleOfficerAssignedCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

}
