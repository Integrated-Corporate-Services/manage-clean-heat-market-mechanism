using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Annual;

/// <summary>
/// Handler for copying files for Annual Boiler Sales command
/// </summary>
public class EditAnnualBoilerSalesCopyFilesCommandHandler : BaseRequestHandler<EditAnnualBoilerSalesCopyFilesCommand, ActionResult>
{
    private readonly IBoilerSalesFileCopyService _fileService;
    private readonly IAnnualBoilerSalesRepository _annualBoilerSalesRepository;

    private readonly IRequestValidator _requestValidator;

    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="fileService">Provides read/write access to files</param>
    /// <param name="annualBoilerSalesRepository">Repository for annual boiler sales data</param>
    /// <param name="requestValidator"></param>
    public EditAnnualBoilerSalesCopyFilesCommandHandler(
        ILogger<BaseRequestHandler<EditAnnualBoilerSalesCopyFilesCommand, ActionResult>> logger,
        IBoilerSalesFileCopyService fileService,
        IAnnualBoilerSalesRepository annualBoilerSalesRepository,
        IRequestValidator requestValidator
        ) : base(logger)
    {
        _fileService = fileService;
        _annualBoilerSalesRepository = annualBoilerSalesRepository;
        _requestValidator = requestValidator;
    }

    /// <summary>
    /// Handles copying files for annual boiler sales data
    /// </summary>
    /// <param name="command">Details of submission</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Id of boiler sales record</returns>
    public override async Task<ActionResult> Handle(EditAnnualBoilerSalesCopyFilesCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;
        var schemeYearId = command.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(organisationId: organisationId, schemeYearId: schemeYearId, requireActiveOrganisation: true);
        if (validationError != null)
            return validationError;

        var annualBoilerSales = await _annualBoilerSalesRepository.Get(o =>
            o.OrganisationId == command.OrganisationId &&
            o.SchemeYearId == command.SchemeYearId,
            includeFiles: true,
            includeChanges: false,
            withTracking: true);

        if (annualBoilerSales == null)
            return CannotLoadBoilerSalesData(schemeYearId, organisationId);

        if (annualBoilerSales.Status != BoilerSalesStatus.Submitted && annualBoilerSales.Status != BoilerSalesStatus.Approved)
            return InvalidAnnualBoilerSalesDataStatus(organisationId, annualBoilerSales.Status);

        var results = await _fileService.PrepareForEditing(organisationId, schemeYearId);

        return results.Count > 0 ?
            ExceptionPreparingS3FilesForEditing(results) :
            Responses.Ok();
    }
}