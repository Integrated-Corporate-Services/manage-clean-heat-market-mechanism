using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Commands;

public class CarryOverCreditsCommandHandler : BaseRequestHandler<CarryOverCreditsCommand, ActionResult>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ISchemeYearService _schemeYearService;
    private readonly ILicenceHolderService _licenceHolderService;
    private readonly IObligationService _obligationService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IInstallationCreditRepository _installationCreditRepository;
    private readonly ICreditLedgerCalculator _installationCreditCalculator;

    public CarryOverCreditsCommandHandler(
        ILogger<CarryOverCreditsCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        ISchemeYearService schemeYearService,
        ILicenceHolderService licenceHolderService,
        IObligationService obligationService,
        ITransactionRepository transactionRepository,
        IInstallationCreditRepository installationCreditRepository,
        ICreditLedgerCalculator installationCreditCalculator
        ) : base(logger)
    {
        _dateTimeProvider = dateTimeProvider;
        _schemeYearService = schemeYearService;
        _licenceHolderService = licenceHolderService;
        _obligationService = obligationService;
        _transactionRepository = transactionRepository;
        _installationCreditRepository = installationCreditRepository;
        _installationCreditCalculator = installationCreditCalculator;
    }

    public override async Task<ActionResult> Handle(CarryOverCreditsCommand request, CancellationToken cancellationToken)
    {
        var schemeYearId = request.SchemeYearId;

        if (await WasPreviouslyRunForSchemeYear(schemeYearId))
            return CarryForwardCreditProcessAlreadyRun(schemeYearId);

        var schemeYearHttpResponse = await _schemeYearService.GetSchemeYear(schemeYearId, cancellationToken);
        if (!schemeYearHttpResponse.IsSuccessStatusCode || schemeYearHttpResponse.Result == null)
            return CannotLoadSchemeYear(schemeYearId, schemeYearHttpResponse.Problem);
        var schemeYear = schemeYearHttpResponse.Result;

        var schemeYearConfigurationHttpResponse = await _schemeYearService.GetObligationCalculations(schemeYearId, cancellationToken);
        if (!schemeYearConfigurationHttpResponse.IsSuccessStatusCode || schemeYearConfigurationHttpResponse.Result == null)
            return CannotLoadObligationCalculations(schemeYearId);
        var schemeYearConfiguration = schemeYearConfigurationHttpResponse.Result;

        var nextSchemeYearHttpResponse = await _schemeYearService.GetNextSchemeYear(schemeYearId, cancellationToken);
        if (!nextSchemeYearHttpResponse.IsSuccessStatusCode || nextSchemeYearHttpResponse.Result == null)
            return CannotLoadNextSchemeYear(schemeYearId, nextSchemeYearHttpResponse.Problem);
        var nextSchemeYear = nextSchemeYearHttpResponse.Result;

        var allLicenceHoldersHttpResponse = await _licenceHolderService.GetAll();
        if (!allLicenceHoldersHttpResponse.IsSuccessStatusCode || allLicenceHoldersHttpResponse.Result == null)
            return CannotLoadLicenceHolders(allLicenceHoldersHttpResponse.Problem);
        var allLicenceHolders = allLicenceHoldersHttpResponse.Result;

        var allLicenceHolderLinksHttpResponse = await _licenceHolderService.GetAllLinks();
        if (!allLicenceHolderLinksHttpResponse.IsSuccessStatusCode || allLicenceHolderLinksHttpResponse.Result == null)
            return CannotLoadLicenceHolderLinks(allLicenceHolderLinksHttpResponse.Problem);
        var allLicenceHolderLinks = allLicenceHolderLinksHttpResponse.Result;

        // All obligations returns a 0 entry for organisations without any boiler submissions.
        var allObligationsHttpResponse = await _obligationService.GetSchemeYearObligationTotals(schemeYearId);
        if (!allObligationsHttpResponse.IsSuccessStatusCode || allObligationsHttpResponse.Result == null)
            return CannotLoadObligationTotals(schemeYearId, allObligationsHttpResponse.Problem);
        var allObligations = allObligationsHttpResponse.Result;

        var transactions = new List<Transaction>();

        var licenceHolderInfo = allLicenceHolderLinks.Select(i => new LicenceOwnershipDto
        {
            LicenceHolderId = i.LicenceHolderId,
            OrganisationId = i.OrganisationId,
            StartDate = i.StartDate,
            EndDate = i.EndDate
        });

        var licenceHolderCredits = await _installationCreditRepository.SumCreditsByLicenceHolderAndOrganisation(licenceHolderInfo, schemeYearId);
        var allTransactions = await _transactionRepository.GetTransactions(schemeYearId);

        var creditBalances = _installationCreditCalculator.GenerateCreditBalances(
            allObligations.Select(i => i.OrganisationId).ToList(),
            licenceHolderCredits,
            allTransactions.ToList()
            );

        foreach (var obligation in allObligations)
        {
            var organisationId = obligation.OrganisationId;

            var totalObligation = obligation.Value;
            var creditBalance = creditBalances.SingleOrDefault(i => i.OrganisationId == organisationId);
            var credits = creditBalance?.CreditBalance ?? 0;

            if (credits <= totalObligation) continue;

            var carryOver = _installationCreditCalculator.CalculateCarryOver(credits, totalObligation, schemeYearConfiguration.CreditCarryOverPercentage);
            transactions.Add(new Transaction(schemeYear.Id, organisationId, -carryOver, IdentityConstants.System.SystemUserId, CreditLedgerConstants.TransactionType.CarriedOverToNextYear, _dateTimeProvider.UtcNow));
            transactions.Add(new Transaction(nextSchemeYear.Id, organisationId, +carryOver, IdentityConstants.System.SystemUserId, CreditLedgerConstants.TransactionType.CarriedOverFromPreviousYear, _dateTimeProvider.UtcNow));
        }

        await _transactionRepository.AddTransactions(transactions, saveChanges: true);

        return Responses.NoContent();
    }

    private async Task<bool> WasPreviouslyRunForSchemeYear(Guid schemeYearId)
    {
        var existingTransactions = await _transactionRepository.GetAllTransactions(x =>
            x.SchemeYearId == schemeYearId
            && (
                x.TransactionType == CreditLedgerConstants.TransactionType.CarriedOverToNextYear
            ));
        return existingTransactions?.Any() ?? false;
    }
}
