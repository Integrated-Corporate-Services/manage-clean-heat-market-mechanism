using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly.SupportingEvidence;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Queries.Quarterly.SupportingEvidence;

public class GetQuarterlySupportingEvidenceFileNamesQueryHandler : BaseRequestHandler<GetQuarterlySupportingEvidenceFileNamesQuery, ActionResult<List<string>>>
{
    private readonly IFileService _fileService;
    private readonly IRequestValidator _requestValidator;

    public GetQuarterlySupportingEvidenceFileNamesQueryHandler(
        ILogger<BaseRequestHandler<GetQuarterlySupportingEvidenceFileNamesQuery, ActionResult<List<string>>>> logger,
        IFileService fileService,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult<List<string>>> Handle(GetQuarterlySupportingEvidenceFileNamesQuery query, CancellationToken cancellationToken)
    {
        var organisationId = query.OrganisationId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId);
        if (validationError != null)
            return validationError;

        var filePrefix = $"{organisationId}/{query.SchemeYearId}/{query.SchemeYearQuarterId}";
        if (query.IsEditing) filePrefix += "/edit";

        var fileNames = await _fileService.GetFileNamesAsync(Buckets.QuarterlySupportingEvidence, filePrefix);
        return fileNames;
    }
}
