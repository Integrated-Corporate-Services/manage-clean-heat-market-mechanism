using Desnz.Chmm.BoilerSales.Api.Extensions;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.VerificationStatement;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Annual.VerificationStatement;

public class UploadAnnualVerificationStatementCommandHandler : BaseRequestHandler<UploadAnnualVerificationStatementCommand, ActionResult>
{
    private readonly IRequestValidator _requestValidator;
    private readonly IFileService _fileService;

    public UploadAnnualVerificationStatementCommandHandler(
        ILogger<BaseRequestHandler<UploadAnnualVerificationStatementCommand, ActionResult>> logger,
        IFileService fileService,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult> Handle(UploadAnnualVerificationStatementCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.ManufacturerId;
        var schemeYearId = command.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(organisationId: organisationId, schemeYearId: schemeYearId);
        if (validationError != null)
            return validationError;

        foreach (var file in command.VerificationStatement)
        {
            var fileKey = $"{organisationId}/{command.SchemeYearId}/";
            if (command.IsEditing) fileKey += "edit/";
            fileKey += file.FileName;

            var uploadResponse = await _fileService.UploadFileAsync(Buckets.AnnualVerificationStatement, fileKey, file);
            if (uploadResponse.ValidationError != null)
                return ErrorUploadingFile(uploadResponse.ValidationError);
        }

        return Responses.Ok();
    }
}
