using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Quarterly;

/// <summary>
/// Handler for copying files for Quarterly Boiler Sales command
/// </summary>
public class EditQuarterlyBoilerSalesCopyFilesCommandHandler : BaseRequestHandler<EditQuarterlyBoilerSalesCopyFilesCommand, ActionResult>
{
    private readonly IBoilerSalesFileCopyService _fileService;
    private readonly IQuarterlyBoilerSalesRepository _quarterlyBoilerSalesRepository;
    private readonly IRequestValidator _requestValidator;

    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="fileService">Provides read/write access to files</param>
    /// <param name="quarterlyBoilerSalesRepository">Repository for quarterly boiler sales data</param>
    /// <param name="requestValidator"></param>
    public EditQuarterlyBoilerSalesCopyFilesCommandHandler(
        ILogger<BaseRequestHandler<EditQuarterlyBoilerSalesCopyFilesCommand, ActionResult>> logger,
        IBoilerSalesFileCopyService fileService,
        IQuarterlyBoilerSalesRepository quarterlyBoilerSalesRepository,
        IRequestValidator requestValidator
        ) : base(logger)
    {
        _fileService = fileService;
        _quarterlyBoilerSalesRepository = quarterlyBoilerSalesRepository;
        _requestValidator = requestValidator;
    }

    /// <summary>
    /// Handles copying files for quarterly boiler sales data
    /// </summary>
    /// <param name="command">Details of submission</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Id of boiler sales record</returns>
    public override async Task<ActionResult> Handle(EditQuarterlyBoilerSalesCopyFilesCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;
        var schemeYearId = command.SchemeYearId;
        var schemeYearQuarterId = command.SchemeYearQuarterId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(organisationId: organisationId, schemeYearId: schemeYearId, schemeYearQuarterId: schemeYearQuarterId, requireActiveOrganisation: true);
        if (validationError != null)
            return validationError;

        var quarterlyBoilerSalesList = await _quarterlyBoilerSalesRepository.Get(o =>
            o.OrganisationId == command.OrganisationId &&
            o.SchemeYearId == command.SchemeYearId &&
            o.SchemeYearQuarterId == command.SchemeYearQuarterId,
            includeFiles: true,
            includeChanges: false,
            withTracking: true);

        if (quarterlyBoilerSalesList == null || quarterlyBoilerSalesList.Count == 0)
        {
            return Responses.BadRequest(string.Format("Failed to get quarterly boiler sales data for organisation {0}, year {1} and quarter {2}", new object[]{ organisationId, schemeYearId, schemeYearQuarterId }));
        }

        var quarterlyBoilerSales = quarterlyBoilerSalesList.First();
        if (quarterlyBoilerSales.Status != BoilerSalesStatus.Submitted && quarterlyBoilerSales.Status != BoilerSalesStatus.Approved)
            return InvalidQuarterlyBoilerSalesDataStatus(organisationId, quarterlyBoilerSales.Status);

        var results = await _fileService.PrepareForEditing(organisationId, schemeYearId, schemeYearQuarterId);

        return results.Count > 0 ?
            ExceptionPreparingS3FilesForEditing(results) :
            Responses.Ok();
    }
}