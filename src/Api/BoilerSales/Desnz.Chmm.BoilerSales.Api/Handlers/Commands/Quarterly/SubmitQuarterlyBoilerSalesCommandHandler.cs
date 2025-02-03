using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Extensions;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Obligation.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Quarterly;

/// <summary>
/// Handler for submission of quarterly boiler sales data
/// </summary>
public class SubmitQuarterlyBoilerSalesCommandHandler : BaseRequestHandler<SubmitQuarterlyBoilerSalesCommand, ActionResult<Guid>>
{
    private readonly IFileService _fileService;
    private readonly IQuarterlyBoilerSalesRepository _quarterlyBoilerSalesRepository;
    private readonly IObligationService _obligationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRequestValidator _requestValidator;

    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="logger">Provides access to logging</param>
    /// <param name="fileService">Provides access to files</param>
    /// <param name="quarterlyBoilerSalesRepository">Repository used to read/write boiler sales data</param>
    /// <param name="obligationService"></param>
    public SubmitQuarterlyBoilerSalesCommandHandler(
       ILogger<BaseRequestHandler<SubmitQuarterlyBoilerSalesCommand, ActionResult<Guid>>> logger,
        IFileService fileService,
        IQuarterlyBoilerSalesRepository quarterlyBoilerSalesRepository,
        IObligationService obligationService,
        IDateTimeProvider dateTimeProvider,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _quarterlyBoilerSalesRepository = quarterlyBoilerSalesRepository;
        _obligationService = obligationService;
        _dateTimeProvider = dateTimeProvider;
        _requestValidator = requestValidator;
    }

    /// <summary>
    /// Handles a submission of quarterly boiler sales data
    /// </summary>
    /// <param name="command">Submission details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of boiler sales data</returns>
    public override async Task<ActionResult<Guid>> Handle(SubmitQuarterlyBoilerSalesCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;
        var schemeYearId = command.SchemeYearId;
        var schemeYearQuarterId = command.SchemeYearQuarterId;
        var now = _dateTimeProvider.UtcDateNow;

        CustomValidator<SchemeYearDto> customValidator = new CustomValidator<SchemeYearDto>
        {
            ValidationFunction = i => 
                now < i.Quarters.Single(q => q.Id == schemeYearQuarterId).EndDate ||
                now >= i.BoilerSalesSubmissionEndDate,
            FailureAction = i => { return CannotSubmitQuarterlyBoilerFiguresOutsideWindow(i.Id); }
        };

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId, 
            requireActiveOrganisation: true, 
            schemeYearId: schemeYearId,
            schemeYearQuarterId: schemeYearQuarterId,
            schemeYearValidation: customValidator);
        if (validationError != null)
            return validationError;

        var quarterSales = await _quarterlyBoilerSalesRepository.Get(a => a.OrganisationId == organisationId && a.SchemeYearQuarterId == schemeYearQuarterId);
        if (quarterSales != null && quarterSales.Any())
            return QuarterSalesDataAlreadyExists(organisationId);

        var prefix = $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}";
        var supportingEvidenceFileNames = await _fileService.GetFileNamesAsync(Buckets.QuarterlySupportingEvidence, prefix);

        var supportingEvidence = supportingEvidenceFileNames
            .Select(fileName =>
                new QuarterlyBoilerSalesFile(fileName.BuildObjectKeyForQuarterlyBoilerSales(organisationId, schemeYearId, schemeYearQuarterId), fileName, FileType.SupportingEvidence))
            .ToList();

        var boilerSales = new QuarterlyBoilerSales(organisationId, schemeYearId, schemeYearQuarterId, command.Gas, command.Oil, supportingEvidence);

        await _quarterlyBoilerSalesRepository.Create(boilerSales, saveChanges:false);


        var submitQuarterlyObligationCommand = new CreateQuarterlyObligationCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
            TransactionDate = boilerSales.CreationDate,
            Gas = boilerSales.Gas,
            Oil = boilerSales.Oil
        };

        var httpResponseGenerateObligation = await _obligationService.SubmitQuarterlyObligation(submitQuarterlyObligationCommand);
        if (!httpResponseGenerateObligation.IsSuccessStatusCode)
            return CannotCreateQuarterlyObligation(organisationId, httpResponseGenerateObligation.Problem);
        
        await _quarterlyBoilerSalesRepository.UnitOfWork.SaveChangesAsync();

        return Responses.Created(boilerSales.Id);
    }
}