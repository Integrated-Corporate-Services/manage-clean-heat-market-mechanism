using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Queries;

/// <summary>
/// Query the organisations credit balance
/// </summary>
public class CreditBalanceQueryHandler : BaseRequestHandler<CreditBalanceQuery, ActionResult<CreditBalanceDto>>
{
    private ILicenceHolderService _licenceHolderService;
    private readonly IRequestValidator _requestValidator;
    private readonly ICreditLedgerCalculator _creditLedgerCalculator;
    private readonly IInstallationCreditRepository _creditLedgerRepository;
    private readonly ITransactionRepository _transactionRepository;

    public CreditBalanceQueryHandler(
        ILogger<BaseRequestHandler<CreditBalanceQuery, ActionResult<CreditBalanceDto>>> logger, 
        ILicenceHolderService licenceHolderService,
        ICreditLedgerCalculator creditLedgerCalculator,
        IInstallationCreditRepository creditLedgerRepository,
        ITransactionRepository transactionRepository,
        IRequestValidator requestValidator) : base(logger)
    {
        _licenceHolderService = licenceHolderService;
        _creditLedgerCalculator = creditLedgerCalculator;
        _creditLedgerRepository = creditLedgerRepository;
        _requestValidator = requestValidator;
        _transactionRepository = transactionRepository;
    }

    /// <summary>
    /// Calculates the current credit balance
    /// </summary>
    /// <param name="query">The query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The current balance</returns>
    public override async Task<ActionResult<CreditBalanceDto>> Handle(CreditBalanceQuery query, CancellationToken cancellationToken)
    {
        var organisationId = query.OrganisationId;
        var schemeYearId = query.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId,
            requireActiveOrganisation: true,
            schemeYearId: schemeYearId
            );
        if (validationError != null)
            return validationError;

        var licenceHoldersResponse = await _licenceHolderService.GetLinkedToHistory(organisationId);
        if (licenceHoldersResponse.Result == null || !licenceHoldersResponse.IsSuccessStatusCode)
            return CannotLoadLicenceHolders(licenceHoldersResponse.Problem);

        var licenceHolderInfo = licenceHoldersResponse.Result.Select(i => new LicenceOwnershipDto
        {
            LicenceHolderId = i.LicenceHolderId,
            OrganisationId = organisationId,
            StartDate = i.StartDate,
            EndDate = i.EndDate
        });

        var creditSums = await _creditLedgerRepository.SumCredits(licenceHolderInfo, schemeYearId);
        var transactions = await _transactionRepository.GetTransactions(organisationId, schemeYearId, false);

        var balance = _creditLedgerCalculator.GenerateCreditBalance(
            organisationId,
            creditSums,
            transactions.ToList());

        return new CreditBalanceDto(balance);
    }
}