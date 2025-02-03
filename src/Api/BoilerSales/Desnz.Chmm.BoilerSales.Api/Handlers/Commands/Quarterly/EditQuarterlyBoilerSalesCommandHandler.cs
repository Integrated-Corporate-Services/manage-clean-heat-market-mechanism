using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Obligation.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Quarterly;

/// <summary>
/// Handler for submission of quarterly boiler sales data
/// </summary>
public class EditQuarterlyBoilerSalesCommandHandler : BaseRequestHandler<EditQuarterlyBoilerSalesCommand, ActionResult>
{
    private readonly IBoilerSalesFileCopyService _fileService;
    private readonly IQuarterlyBoilerSalesRepository _quarterlyBoilerSalesRepository;
    private readonly IObligationService _obligationService;

    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="logger">Provides access to logging</param>
    /// <param name="fileService">Provides access to files</param>
    /// <param name="quarterlyBoilerSalesRepository">Repository used to read/write boiler sales data</param>
    /// <param name="obligationService"></param>
    public EditQuarterlyBoilerSalesCommandHandler(
       ILogger<BaseRequestHandler<EditQuarterlyBoilerSalesCommand, ActionResult>> logger,
        IBoilerSalesFileCopyService fileService,
        IQuarterlyBoilerSalesRepository quarterlyBoilerSalesRepository,
        IObligationService obligationService,
        IDateTimeProvider dateTimeProvider,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _quarterlyBoilerSalesRepository = quarterlyBoilerSalesRepository;
        _obligationService = obligationService;
    }

    /// <summary>
    /// Handles a submission of quarterly boiler sales data
    /// </summary>
    /// <param name="command">Submission details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of boiler sales data</returns>

    public override async Task<ActionResult> Handle(EditQuarterlyBoilerSalesCommand command, CancellationToken cancellationToken)
    {
        var quarterSalesList = await _quarterlyBoilerSalesRepository.Get(
            a =>
                a.OrganisationId == command.OrganisationId &&
                a.SchemeYearId == command.SchemeYearId &&
                a.SchemeYearQuarterId == command.SchemeYearQuarterId,
            includeFiles: true,
            includeChanges: false,
            withTracking: true);

        if (quarterSalesList == null || quarterSalesList.Count == 0)
            return CannotLoadBoilerSalesData(command.SchemeYearId, command.SchemeYearQuarterId, command.OrganisationId);

        var quarterlyBoilerSales = quarterSalesList.First();


        if (quarterlyBoilerSales.Status != BoilerSalesStatus.Submitted && quarterlyBoilerSales.Status != BoilerSalesStatus.Approved)
            return InvalidQuarterlyBoilerSalesDataStatus(quarterlyBoilerSales.OrganisationId, quarterlyBoilerSales.Status);


        var prefix = $"{quarterlyBoilerSales.OrganisationId}/{quarterlyBoilerSales.SchemeYearId}/{quarterlyBoilerSales.SchemeYearQuarterId}";
        var concludedEditingSession = await _fileService.ConcludeEditing(quarterlyBoilerSales.OrganisationId, quarterlyBoilerSales.SchemeYearId, quarterlyBoilerSales.SchemeYearQuarterId);

        if (concludedEditingSession.Errors.Any())
            return ExceptionConcludingS3FilesForEditing(concludedEditingSession.Errors);

        var supportingEvidenceFileNames = concludedEditingSession.SupportingEvidences;
        var supportingEvidence = supportingEvidenceFileNames
            .Select(fileName =>
                new QuarterlyBoilerSalesFile($"{prefix}({fileName}", fileName, FileType.SupportingEvidence))
            .ToList();

        quarterlyBoilerSales.Edit(command.Gas, command.Oil, supportingEvidence);

        await _quarterlyBoilerSalesRepository.Update(quarterlyBoilerSales, true);

        var submitAnnualObligationCommand = new CreateQuarterlyObligationCommand
        {
            OrganisationId = quarterlyBoilerSales.OrganisationId,
            SchemeYearId = quarterlyBoilerSales.SchemeYearId,
            SchemeYearQuarterId = quarterlyBoilerSales.SchemeYearQuarterId,
            TransactionDate = quarterlyBoilerSales.CreationDate,
            Gas = quarterlyBoilerSales.Gas,
            Oil = quarterlyBoilerSales.Oil,
            Override = true
        };

        var httpResponseGenerateObligation = await _obligationService.SubmitQuarterlyObligation(submitAnnualObligationCommand);
        if (!httpResponseGenerateObligation.IsSuccessStatusCode)
        {
            return CannotCreateQuarterlyObligation(quarterlyBoilerSales.OrganisationId, httpResponseGenerateObligation.Problem);
        }

        await _quarterlyBoilerSalesRepository.UnitOfWork.SaveChangesAsync();

        return Responses.Ok();
    }
}