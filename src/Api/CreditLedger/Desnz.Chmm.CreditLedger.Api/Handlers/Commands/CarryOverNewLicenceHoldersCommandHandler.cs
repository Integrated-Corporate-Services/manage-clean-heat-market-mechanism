using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.CreditLedger.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Commands;

public class CarryOverNewLicenceHoldersCommandHandler : BaseRequestHandler<CarryOverNewLicenceHoldersCommand, ActionResult>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ISchemeYearService _schemeYearService;
    private readonly ILicenceHolderService _licenceHolderService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IInstallationCreditRepository _installationCreditRepository;
    private readonly ICreditLedgerCalculator _installationCreditCalculator;
    private readonly IRequestValidator _requestValidator;

    public CarryOverNewLicenceHoldersCommandHandler(
        ILogger<CarryOverNewLicenceHoldersCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        ISchemeYearService schemeYearService,
        ILicenceHolderService licenceHolderService,
        ITransactionRepository transactionRepository,
        IInstallationCreditRepository installationCreditRepository,
        ICreditLedgerCalculator installationCreditCalculator,
        IRequestValidator requestValidator
        ) : base(logger)
    {
        _dateTimeProvider = dateTimeProvider;
        _schemeYearService = schemeYearService;
        _licenceHolderService = licenceHolderService;
        _transactionRepository = transactionRepository;
        _installationCreditRepository = installationCreditRepository;
        _installationCreditCalculator = installationCreditCalculator;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult> Handle(CarryOverNewLicenceHoldersCommand command, CancellationToken cancellationToken)
    {
        var licenceHolderExistsHttpResponse = await _licenceHolderService.Exists(command.LicenceHolderId);
        if (!licenceHolderExistsHttpResponse.IsSuccessStatusCode || licenceHolderExistsHttpResponse.Result == null)
            return CannotLoadLicenceHolders(licenceHolderExistsHttpResponse.Problem);
        if (!licenceHolderExistsHttpResponse.Result.Exists)
            return CannotLoadLicenceHolder(command.LicenceHolderId);

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: command.OrganisationId,
            requireActiveOrganisation: true,
            schemeYearId: command.PreviousSchemeYearId);
        if (validationError != null)
            return validationError;

        var schemeYearResponse = await _schemeYearService.GetCurrentSchemeYear(cancellationToken);
        if (!schemeYearResponse.IsSuccessStatusCode || schemeYearResponse.Result == null)
            return CannotLoadCurrentSchemeYear(schemeYearResponse.Problem);
        var currentSchemeYear = schemeYearResponse.Result;

        if (command.StartDate >= currentSchemeYear.SurrenderDayDate)
            return Responses.NoContent();

        var credits = await _installationCreditRepository.GetAll(x => x.LicenceHolderId == command.LicenceHolderId &&
                                                    x.SchemeYearId == command.PreviousSchemeYearId &&
                                                    x.DateCreditGenerated > command.StartDate);
        if (credits.Count == 0)
            return Responses.NoContent();

        var previousSchemeYearConfigurationHttpResponse = await _schemeYearService.GetObligationCalculations(command.PreviousSchemeYearId, cancellationToken);
        if (!previousSchemeYearConfigurationHttpResponse.IsSuccessStatusCode || previousSchemeYearConfigurationHttpResponse.Result == null)
            return CannotLoadObligationCalculations(currentSchemeYear.Id);
        var previousSchemeYearConfiguration = previousSchemeYearConfigurationHttpResponse.Result;

        var carryOver = _installationCreditCalculator.CalculateNewLicenceHoldersCarryOver(credits.Sum(x => x.Value), previousSchemeYearConfiguration.CreditCarryOverPercentage);

        if(carryOver ==  0)
            return Responses.NoContent();

        var transactions = new List<Transaction>
        {
            new Transaction(command.PreviousSchemeYearId, command.OrganisationId, -carryOver, IdentityConstants.System.SystemUserId, CreditLedgerConstants.TransactionType.CarriedOverToNextYear, _dateTimeProvider.UtcNow),
            new Transaction(currentSchemeYear.Id, command.OrganisationId, +carryOver, IdentityConstants.System.SystemUserId, CreditLedgerConstants.TransactionType.CarriedOverFromPreviousYear, _dateTimeProvider.UtcNow)
        };

        await _transactionRepository.AddTransactions(transactions, saveChanges: true);

        return Responses.NoContent();
    }
}
