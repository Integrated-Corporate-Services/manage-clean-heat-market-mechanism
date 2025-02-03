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
using Desnz.Chmm.CreditLedger.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Commands;

public class AdjustCreditsCommandHandler : BaseRequestHandler<AdjustCreditsCommand, ActionResult>
{
    private ICurrentUserService _userService;
    private IDateTimeProvider _dateTimeProvider;
    private ITransactionRepository _transactionRepository;
    private ILicenceHolderService _licenceHolderService;
    private readonly IInstallationCreditRepository _creditLedgerRepository;
    private readonly IRequestValidator _requestValidator;
    private readonly ICreditLedgerCalculator _creditLedgerCalculator;

    /// <summary>
    /// Default constructor
    /// </summary>

    public AdjustCreditsCommandHandler(
        ILogger<BaseRequestHandler<AdjustCreditsCommand, ActionResult>> logger,
        ICurrentUserService userService,
        IDateTimeProvider dateTimeProvider,
        ITransactionRepository transactionRepository,
        ILicenceHolderService licenceHolderService,
        IInstallationCreditRepository creditLedgerRepository,
        IRequestValidator requestValidator,
        ICreditLedgerCalculator creditLedgerCalculator) : base(logger)
    {
        _userService = userService;
        _dateTimeProvider = dateTimeProvider;
        _transactionRepository = transactionRepository;
        _licenceHolderService = licenceHolderService;
        _creditLedgerRepository = creditLedgerRepository;
        _requestValidator = requestValidator;
        _creditLedgerCalculator = creditLedgerCalculator;
    }

    public override async Task<ActionResult> Handle(AdjustCreditsCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;
        var schemeYearId = command.SchemeYearId;
        var now = _dateTimeProvider.UtcDateNow;

        CustomValidator<SchemeYearDto> customValidator = new CustomValidator<SchemeYearDto>
        {
            ValidationFunction = i => now < i.StartDate || now >= i.SurrenderDayDate,
            FailureAction = i => { return CannotAdjustCreditsOutsideAdjustmentWindow(i.Id); }
        };

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId,
            requireActiveOrganisation: true,
            schemeYearId: schemeYearId,
            schemeYearValidation: customValidator);
        if (validationError != null)
            return validationError;

        ClaimsPrincipal currentUser = _userService.CurrentUser;

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

        var creditSums = await _creditLedgerRepository.SumCredits(licenceHolders, schemeYearId);
        var transactions = await _transactionRepository.GetTransactions(organisationId, schemeYearId, false);

        var totalToDate = _creditLedgerCalculator.GenerateCreditBalance(
           organisationId,
           creditSums,
           transactions.ToList());

        if (totalToDate + command.Value < 0)
            return NotEnoughCredits(totalToDate);

        var transaction = new Transaction(schemeYearId, organisationId, command.Value, currentUser.GetUserId().Value, CreditLedgerConstants.TransactionType.AdminAdjustment, _dateTimeProvider.UtcNow);
        await _transactionRepository.AddTransaction(transaction);

        return Responses.Created(transaction.Id);
    }
}