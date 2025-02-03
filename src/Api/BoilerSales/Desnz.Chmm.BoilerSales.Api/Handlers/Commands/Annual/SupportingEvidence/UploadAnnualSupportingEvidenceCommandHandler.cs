using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Extensions;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Annual.SupportingEvidence;

public class UploadAnnualSupportingEvidenceCommandHandler : BaseRequestHandler<UploadAnnualSupportingEvidenceCommand, ActionResult>
{
    private readonly IRequestValidator _requestValidator;
    private readonly IFileService _fileService;

    public UploadAnnualSupportingEvidenceCommandHandler(
        ILogger<BaseRequestHandler<UploadAnnualSupportingEvidenceCommand, ActionResult>> logger,
        IFileService fileService,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult> Handle(UploadAnnualSupportingEvidenceCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;
        var schemeYearId = command.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(organisationId: organisationId, schemeYearId: schemeYearId);
        if (validationError != null)
            return validationError;

        foreach (var file in command.SupportingEvidence)
        {
            var fileKey = $"{organisationId}/{command.SchemeYearId}/";
            if (command.IsEditing) fileKey += "edit/";
            fileKey += file.FileName;

            var uploadResponse = await _fileService.UploadFileAsync(Buckets.AnnualSupportingEvidence, fileKey, file);
            if (uploadResponse.ValidationError != null)
                return Responses.BadRequest(uploadResponse.ValidationError);
        }

        return Responses.Ok();
    }
}
