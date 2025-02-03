using Desnz.Chmm.BoilerSales.Api.Extensions;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Annual.SupportingEvidence;

public class DeleteAnnualSupportingEvidenceCommandHandler : BaseRequestHandler<DeleteAnnualSupportingEvidenceCommand, ActionResult>
{
    private readonly IFileService _fileService;
    private readonly IRequestValidator _requestValidator;

    public DeleteAnnualSupportingEvidenceCommandHandler(
        ILogger<BaseRequestHandler<DeleteAnnualSupportingEvidenceCommand, ActionResult>> logger,
        IFileService fileService,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult> Handle(DeleteAnnualSupportingEvidenceCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(organisationId: organisationId);
        if (validationError != null)
            return validationError;

        var fileKey = $"{organisationId}/{command.SchemeYearId}/";
        if (command.IsEditing) fileKey += "edit/";
        fileKey += command.FileName;

        var deleteResponse = await _fileService.DeleteObjectNonVersionedBucketAsync(Buckets.AnnualSupportingEvidence, fileKey);
        if (deleteResponse.ValidationError != null)
            return ErrorDeletingFile(deleteResponse.ValidationError);

        return Responses.Ok();
    }
}
