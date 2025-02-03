using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Queries;

public class GetAllCreditBalancesQueryHandler : BaseRequestHandler<GetAllCreditBalancesQuery, ActionResult<List<OrganisationCreditBalanceDto>>>
{
    private readonly IOrganisationService _organisationService;
    private readonly ILicenceHolderService _licenceHolderService;
    private readonly IRequestValidator _requestValidator;
    private readonly ICreditLedgerCalculator _installationCreditCalculator;
    private readonly IInstallationCreditRepository _installationCreditRepository;
    private readonly ITransactionRepository _transactionRepository;

    public GetAllCreditBalancesQueryHandler(
        ILogger<BaseRequestHandler<GetAllCreditBalancesQuery, ActionResult<List<OrganisationCreditBalanceDto>>>> logger,
        IOrganisationService organisationService,
        ILicenceHolderService licenceHolderService,
        IInstallationCreditRepository installationCreditRepository,
        ITransactionRepository transactionRepository,
        ICreditLedgerCalculator installationCreditCalculator,
        IRequestValidator requestValidator) : base(logger)
    {
        _organisationService = organisationService;
        _licenceHolderService = licenceHolderService;
        _installationCreditRepository = installationCreditRepository;
        _transactionRepository = transactionRepository;
        _installationCreditCalculator = installationCreditCalculator;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult<List<OrganisationCreditBalanceDto>>> Handle(GetAllCreditBalancesQuery query, CancellationToken cancellationToken)
    {
        var schemeYearId = query.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            schemeYearId: schemeYearId
            );
        if (validationError != null)
            return validationError;

        var organisationResponse = await _organisationService.GetManufacturers();
        if (organisationResponse.Result == null || !organisationResponse.IsSuccessStatusCode)
            return CannotLoadOrganisations(organisationResponse.Problem);

        var allLicenceHolderLinksHttpResponse = await _licenceHolderService.GetAllLinks();
        if (!allLicenceHolderLinksHttpResponse.IsSuccessStatusCode || allLicenceHolderLinksHttpResponse.Result == null)
            return CannotLoadLicenceHolderLinks(allLicenceHolderLinksHttpResponse.Problem);
        var allLicenceHolderLinks = allLicenceHolderLinksHttpResponse.Result;

        var licenceHolderInfo = allLicenceHolderLinks.Select(i => new LicenceOwnershipDto
        {
            LicenceHolderId = i.LicenceHolderId,
            OrganisationId = i.OrganisationId,
            StartDate = i.StartDate,
            EndDate = i.EndDate
        });

        var licenceHolderCredits = await _installationCreditRepository.SumCreditsByLicenceHolderAndOrganisation(licenceHolderInfo, schemeYearId);
        var allTransactions = await _transactionRepository.GetTransactions(schemeYearId);

        var list = _installationCreditCalculator.GenerateCreditBalances(
            organisationResponse.Result.Select(i => i.Id).ToList(),
            licenceHolderCredits,
            allTransactions.ToList()
            );

        return list;
    }
}
