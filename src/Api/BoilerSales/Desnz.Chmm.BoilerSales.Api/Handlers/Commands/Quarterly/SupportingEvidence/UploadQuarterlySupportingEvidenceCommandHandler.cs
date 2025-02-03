using Desnz.Chmm.BoilerSales.Api.Extensions;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly.SupportingEvidence;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Quarterly.SupportingEvidence;

public class UploadQuarterlySupportingEvidenceCommandHandler : BaseRequestHandler<UploadQuarterlySupportingEvidenceCommand, ActionResult>
{
    private readonly IRequestValidator _requestValidator;
    private readonly IFileService _fileService;

    public UploadQuarterlySupportingEvidenceCommandHandler(
        ILogger<BaseRequestHandler<UploadQuarterlySupportingEvidenceCommand, ActionResult>> logger,
        IFileService fileService,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult> Handle(UploadQuarterlySupportingEvidenceCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(organisationId: organisationId);
        if (validationError != null)
            return validationError;

        foreach (var file in command.SupportingEvidence)
        {
            var fileKey = $"{organisationId}/{command.SchemeYearId}/{command.SchemeYearQuarterId}/";
            if (command.IsEditing) fileKey += "edit/";
            fileKey += file.FileName;

            var uploadResponse = await _fileService.UploadFileAsync(Buckets.QuarterlySupportingEvidence, fileKey, file);
            if (uploadResponse.ValidationError != null)
                return Responses.BadRequest(uploadResponse.ValidationError);
        }

        return Responses.Ok();
    }
}
