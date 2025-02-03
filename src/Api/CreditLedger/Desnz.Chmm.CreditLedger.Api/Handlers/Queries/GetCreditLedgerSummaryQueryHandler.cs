using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Queries;

public class GetCreditLedgerSummaryQueryHandler : BaseRequestHandler<GetCreditLedgerSummaryQuery, ActionResult<CreditLedgerSummaryDto>>
{
    private readonly IInstallationCreditRepository _creditLedgerRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILicenceHolderService _licenceHolderService;
    private readonly ICreditLedgerCalculator _installationCreditCalculator;
    private readonly IRequestValidator _requestValidator;

    public GetCreditLedgerSummaryQueryHandler(
        ILogger<BaseRequestHandler<GetCreditLedgerSummaryQuery, ActionResult<CreditLedgerSummaryDto>>> logger,
        IInstallationCreditRepository creditLedgerRepository,
        ITransactionRepository transactionRepository,
        ILicenceHolderService licenceHolderService,
        ICreditLedgerCalculator installationCreditCalculator,
        IRequestValidator requestValidator) : base(logger)
    {
        _creditLedgerRepository = creditLedgerRepository;
        _transactionRepository = transactionRepository;
        _licenceHolderService = licenceHolderService;
        _installationCreditCalculator = installationCreditCalculator;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult<CreditLedgerSummaryDto>> Handle(GetCreditLedgerSummaryQuery query, CancellationToken cancellationToken)
    {
        var organisationId = query.OrganisationId;
        var schemeYearId = query.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId,
            requireActiveOrganisation: true,
            schemeYearId: schemeYearId);
        if (validationError != null)
            return validationError;

        var licenceHoldersResponse = await _licenceHolderService.GetLinkedToHistory(organisationId);
        if (!licenceHoldersResponse.IsSuccessStatusCode || licenceHoldersResponse.Result == null)
            return CannotLoadLicenceHolders(organisationId, licenceHoldersResponse.Problem);

        var licenceHolders = licenceHoldersResponse.Result.Select(i => new LicenceOwnershipDto
        {
            LicenceHolderId = i.LicenceHolderId,
            OrganisationId = organisationId,
            StartDate = i.StartDate,
            EndDate = i.EndDate
        });

        var installationCredits = await _creditLedgerRepository.GetInstallationCredits(licenceHolders, schemeYearId);
        var transactions = await _transactionRepository.GetTransactions(organisationId, schemeYearId);

        var summary = _installationCreditCalculator.GenerateCreditLedgerSummary(organisationId, installationCredits, transactions);

        return summary;
    }
}