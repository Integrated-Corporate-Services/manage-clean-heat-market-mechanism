using Desnz.Chmm.BoilerSales.Common.Queries.Annual.VerificationStatement;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Queries.Annual.VerificationStatement;

public class GetAnnualVerificationStatementFileNamesQueryHandler : BaseRequestHandler<GetAnnualVerificationStatementFileNamesQuery, ActionResult<List<string>>>
{
    private readonly IFileService _fileService;
    private readonly IRequestValidator _requestValidator;

    public GetAnnualVerificationStatementFileNamesQueryHandler(
        ILogger<BaseRequestHandler<GetAnnualVerificationStatementFileNamesQuery, ActionResult<List<string>>>> logger,
        IFileService fileService,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult<List<string>>> Handle(GetAnnualVerificationStatementFileNamesQuery query, CancellationToken cancellationToken)
    {
        var organisationId = query.OrganisationId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId);
        if (validationError != null)
            return validationError;

        var prefix = $"{organisationId}/{query.SchemeYearId}";
        if (query.IsEditing) prefix += "/edit";

        return await _fileService.GetFileNamesAsync(Buckets.AnnualVerificationStatement, prefix);
    }
}
