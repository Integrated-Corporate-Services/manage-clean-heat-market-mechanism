using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Obligation.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Annual;

/// <summary>
/// Handler for editing Annual Boiler Sales command
/// </summary>
public class EditAnnualBoilerSalesCommandHandler : BaseRequestHandler<EditAnnualBoilerSalesCommand, ActionResult>
{
    private readonly IBoilerSalesFileCopyService _fileService;
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
    public EditAnnualBoilerSalesCommandHandler(
        ILogger<BaseRequestHandler<EditAnnualBoilerSalesCommand, ActionResult>> logger,
        IBoilerSalesFileCopyService fileService,
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
    /// Handles editing of annual boiler sales data
    /// </summary>
    /// <param name="command">Details of submission</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Id of boiler sales record</returns>
    public override async Task<ActionResult> Handle(EditAnnualBoilerSalesCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;
        var schemeYearId = command.SchemeYearId;
        var now = _dateTimeProvider.UtcDateNow;

        CustomValidator<SchemeYearDto> customValidator = new CustomValidator<SchemeYearDto>
        {
            ValidationFunction = i => now < i.EndDate || now >= i.SurrenderDayDate,
            FailureAction = i => { return CannotEditAnnualBoilerFiguresOutsideWindow(i.Id); }
        };

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId, 
            schemeYearId: schemeYearId,
            requireActiveOrganisation: true,
            schemeYearValidation: customValidator);
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

        var prefix = $"{organisationId}/{schemeYearId}";

        var concludedEditingSession = await _fileService.ConcludeEditing(organisationId, schemeYearId);

        if (concludedEditingSession.Errors.Any())
            return ExceptionConcludingS3FilesForEditing(concludedEditingSession.Errors);

        var verificationStatementFileNames = concludedEditingSession.VerificationStatements;
        var annualBoilerSalesFiles = verificationStatementFileNames
            .Select(fileName =>
                new AnnualBoilerSalesFile($"{prefix}({fileName}", fileName, FileType.VerificationStatement))
            .ToList();

        var supportingEvidenceFileNames = concludedEditingSession.SupportingEvidences;
        var supportingEvidence = supportingEvidenceFileNames
            .Select(fileName =>
                new AnnualBoilerSalesFile($"{prefix}({fileName}", fileName, FileType.SupportingEvidence))
            .ToList();

        annualBoilerSalesFiles.AddRange(supportingEvidence);

        annualBoilerSales.Edit(command.Gas, command.Oil, annualBoilerSalesFiles);

        await _annualBoilerSalesRepository.Update(annualBoilerSales, true);

        var submitAnnualObligationCommand = new CreateAnnualObligationCommand
        {
            OrganisationId = command.OrganisationId,
            SchemeYearId = command.SchemeYearId,
            TransactionDate = annualBoilerSales.CreationDate,
            Gas = annualBoilerSales.Gas,
            Oil = annualBoilerSales.Oil,
            Override = true
        };

        var httpResponseGenerateObligation = await _obligationService.SubmitAnnualObligation(submitAnnualObligationCommand);
        if (!httpResponseGenerateObligation.IsSuccessStatusCode)
        {
            return CannotCreateYearlyObligation(organisationId, httpResponseGenerateObligation.Problem);
        }

        return Responses.Ok();
    }
}