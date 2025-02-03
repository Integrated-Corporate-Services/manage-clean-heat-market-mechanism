using Desnz.Chmm.BoilerSales.Api.Extensions;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly.SupportingEvidence;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Quarterly.SupportingEvidence;

public class DeleteQuarterlySupportingEvidenceCommandHandler : BaseRequestHandler<DeleteQuarterlySupportingEvidenceCommand, ActionResult>
{
    private readonly IFileService _fileService;
    private readonly IRequestValidator _requestValidator;

    public DeleteQuarterlySupportingEvidenceCommandHandler(
        ILogger<BaseRequestHandler<DeleteQuarterlySupportingEvidenceCommand, ActionResult>> logger,
        IFileService fileService,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult> Handle(DeleteQuarterlySupportingEvidenceCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(organisationId: organisationId);
        if (validationError != null)
            return validationError;

        var fileKey = $"{organisationId}/{command.SchemeYearId}/{command.SchemeYearQuarterId}/";
        if (command.IsEditing) fileKey += "edit/";
        fileKey += command.FileName;

        var deleteResponse = await _fileService.DeleteObjectNonVersionedBucketAsync(Buckets.QuarterlySupportingEvidence, fileKey);
        if (deleteResponse.ValidationError != null)
            return ErrorDeletingFile(deleteResponse.ValidationError);

        return Responses.Ok();
    }
}
