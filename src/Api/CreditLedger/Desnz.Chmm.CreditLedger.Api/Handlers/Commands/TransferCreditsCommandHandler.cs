using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.Identity.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Commands;

/// <summary>
/// Handle tranfering credits between manufacturers
/// </summary>
public class TransferCreditsCommandHandler : BaseRequestHandler<TransferCreditsCommand, ActionResult>
{
    private readonly ICurrentUserService _userService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IInstallationCreditRepository _creditLedgerRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILicenceHolderService _licenceHolderService;
    private readonly IOrganisationService _organisationService;
    private readonly ICreditLedgerCalculator _creditCalculator;
    private readonly IRequestValidator _requestValidator;

    /// <summary>
    /// Default constructor
    /// </summary>

    public TransferCreditsCommandHandler(
        ILogger<BaseRequestHandler<TransferCreditsCommand, ActionResult>> logger, 
        ICurrentUserService userService,
        IDateTimeProvider dateTimeProvider,
        IInstallationCreditRepository creditLedgerRepository, 
        ITransactionRepository transactionRepository, 
        ILicenceHolderService licenceHolderService,
        IOrganisationService organisationService,
        ICreditLedgerCalculator creditCalculator,
        IRequestValidator requestValidator) : base(logger)
    {
        _userService = userService;
        _dateTimeProvider = dateTimeProvider;
        _creditLedgerRepository = creditLedgerRepository;
        _transactionRepository = transactionRepository;
        _licenceHolderService = licenceHolderService;
        _organisationService = organisationService;
        _creditCalculator = creditCalculator;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult> Handle(TransferCreditsCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;
        var destinationOrganisationId = command.DestinationOrganisationId;
        var schemeYearId = command.SchemeYearId;
        var now = _dateTimeProvider.UtcDateNow;

        if (organisationId == destinationOrganisationId)
        {
            return CannotTransferCreditsToSameOrganisation(organisationId);
        }

        CustomValidator<SchemeYearDto> customValidator = new CustomValidator<SchemeYearDto>
        {
            ValidationFunction = i => now < i.TradingWindowStartDate || now > i.TradingWindowEndDate,
            FailureAction = i => { return CannotTradeOutsideWindow(i.Id); }
        };

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId,
            requireActiveOrganisation: true,
            schemeYearId: schemeYearId,
            schemeYearValidation: customValidator);
        if (validationError != null)
            return validationError;

        var availableForTransferResponse = await _organisationService.GetOrganisationsAvailableForTransfer(organisationId);
        if (availableForTransferResponse.Result == null || !availableForTransferResponse.IsSuccessStatusCode)
            return CannotLoadOrganisations(availableForTransferResponse.Problem);
        if (!availableForTransferResponse.Result.Any(i => i.Id == destinationOrganisationId))
            return CannotTransferToOrganisation(destinationOrganisationId);
        var destinationOrg = availableForTransferResponse.Result.Single(i => i.Id == destinationOrganisationId);
        if (destinationOrg.Status != OrganisationConstants.Status.Active)
            return InvalidOrganisationStatus(destinationOrganisationId, destinationOrg.Status);

        var licenceHoldersResponse = await _licenceHolderService.GetLinkedToHistory(organisationId);
        if (licenceHoldersResponse.Result == null || !licenceHoldersResponse.IsSuccessStatusCode)
            return CannotLoadLicenceHolders(licenceHoldersResponse.Problem);

        var licenceHolders = licenceHoldersResponse.Result.Select(i => new LicenceOwnershipDto
        {
            LicenceHolderId = i.LicenceHolderId,
            OrganisationId = organisationId,
            StartDate = i.StartDate,
            EndDate = i.EndDate
        });

        var creditSum = await _creditLedgerRepository.SumCredits(licenceHolders, schemeYearId);
        var transactions = await _transactionRepository.GetTransactions(organisationId, schemeYearId);

        var totalBalance = _creditCalculator.GenerateCreditBalance(organisationId, creditSum, transactions.ToList());

        if (totalBalance < command.Value)
            return NotEnoughCredits(totalBalance);

        var creditTransfer = new CreditTransfer(organisationId, destinationOrganisationId, schemeYearId, _userService.CurrentUser.GetUserId().Value, command.Value);

        // By default, currently we are just accepting all credit transfer requests
        creditTransfer.AcceptTransfer();

        await _creditLedgerRepository.AddCreditTransfer(creditTransfer);

        return Responses.Created(creditTransfer.Id);
    }
}