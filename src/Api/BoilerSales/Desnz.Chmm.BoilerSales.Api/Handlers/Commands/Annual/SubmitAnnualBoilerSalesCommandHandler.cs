using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Extensions;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Obligation.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Annual;

/// <summary>
/// Handler for Submit Annual Boiler Sales command
/// </summary>
public class SubmitAnnualBoilerSalesCommandHandler : BaseRequestHandler<SubmitAnnualBoilerSalesCommand, ActionResult<Guid>>
{
    private readonly IFileService _fileService;
    private readonly IAnnualBoilerSalesRepository _annualBoilerSalesRepository;
    private readonly IObligationService _obligationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRequestValidator _requestValidator;

    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="fileService">Provides read/write access to files</param>
    /// <param name="annualBoilerSalesRepository">Repository for annual boiler sales data</param>
    /// <param name="obligationService">Obligation service</param>
    public SubmitAnnualBoilerSalesCommandHandler(
        ILogger<BaseRequestHandler<SubmitAnnualBoilerSalesCommand, ActionResult<Guid>>> logger,
        IFileService fileService,
        IAnnualBoilerSalesRepository annualBoilerSalesRepository,
        IObligationService obligationService,
        IDateTimeProvider dateTimeProvider,
        IRequestValidator requestValidator
        ) : base(logger)
    {
        _fileService = fileService;
        _annualBoilerSalesRepository = annualBoilerSalesRepository;
        _obligationService = obligationService;
        _dateTimeProvider = dateTimeProvider;
        _requestValidator = requestValidator;
    }

    /// <summary>
    /// Handles submission of annual boiler sales data
    /// </summary>
    /// <param name="command">Details of submission</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Id of boiler sales record</returns>
    public override async Task<ActionResult<Guid>> Handle(SubmitAnnualBoilerSalesCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;
        var schemeYearId = command.SchemeYearId;
        var now = _dateTimeProvider.UtcDateNow;

        CustomValidator<SchemeYearDto> customValidator = new CustomValidator<SchemeYearDto>
        {
            ValidationFunction = i => now < i.EndDate || now > i.BoilerSalesSubmissionEndDate,
            FailureAction = i => { return CannotSubmitAnnualBoilerFiguresOutsideWindow(i.Id); }
        };

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId,
            requireActiveOrganisation: true,
            schemeYearId: schemeYearId,
            schemeYearValidation: customValidator);
        if (validationError != null)
            return validationError;

        var annualSales = await _annualBoilerSalesRepository.Get(a => a.OrganisationId == organisationId && a.SchemeYearId == schemeYearId);
        if (annualSales != null)
            return AnnualSalesDataAlreadyExists(organisationId);

        var prefix = $"{organisationId}/{schemeYearId}";

        // Run these two tasks in parallel to save some time
        var verificationStatementTask = Task.Run(() => _fileService.GetFileNamesAsync(Buckets.AnnualVerificationStatement, prefix));
        var supportingEvidenceTask = Task.Run(() => _fileService.GetFileNamesAsync(Buckets.AnnualSupportingEvidence, prefix));

        await Task.WhenAll(verificationStatementTask, supportingEvidenceTask);

        var verificationStatementFileNames = verificationStatementTask.Result;
        var annualBoilerSalesFiles = verificationStatementFileNames
            .Select(fileName =>
                new AnnualBoilerSalesFile(fileName.BuildObjectKeyForAnnualBoilerSales(organisationId, schemeYearId), fileName, FileType.VerificationStatement))
            .ToList();

        var supportingEvidenceFileNames = supportingEvidenceTask.Result;
        var supportingEvidence = supportingEvidenceFileNames
            .Select(fileName =>
                new AnnualBoilerSalesFile(fileName.BuildObjectKeyForAnnualBoilerSales(organisationId, schemeYearId), fileName, FileType.SupportingEvidence))
            .ToList();

        annualBoilerSalesFiles.AddRange(supportingEvidence);
        var boilerSales = new AnnualBoilerSales(schemeYearId, command.OrganisationId, command.Gas, command.Oil, annualBoilerSalesFiles);

        var boilerSalesId = await _annualBoilerSalesRepository.Create(boilerSales, true);

        var submitAnnualObligationCommand = new CreateAnnualObligationCommand
        {
            OrganisationId = command.OrganisationId,
            SchemeYearId = command.SchemeYearId,
            TransactionDate = boilerSales.CreationDate,
            Gas = boilerSales.Gas,
            Oil = boilerSales.Oil
        };

        var httpResponseGenerateObligation = await _obligationService.SubmitAnnualObligation(submitAnnualObligationCommand);
        if (!httpResponseGenerateObligation.IsSuccessStatusCode)
        {
            await _annualBoilerSalesRepository.Delete(boilerSales, true);
            return CannotCreateYearlyObligation(organisationId, httpResponseGenerateObligation.Problem);
        }

        return Responses.Created(boilerSales.Id);
    }
}