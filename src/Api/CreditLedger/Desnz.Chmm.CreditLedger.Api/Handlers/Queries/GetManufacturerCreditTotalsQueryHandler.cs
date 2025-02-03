using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Queries;

public class GetManufacturerCreditTotalsQueryHandler : BaseRequestHandler<GetManufacturerCreditTotalsQuery, ActionResult<List<PeriodCreditTotals>>>
{
    private readonly IInstallationCreditRepository _creditLedgerRepository;
    private readonly ILicenceHolderService _licenceHolderService;
    private readonly IRequestValidator _requestValidator;
    private readonly IMcsSynchronisationService _mcsSynchronisationService;

    public GetManufacturerCreditTotalsQueryHandler(
        ILogger<BaseRequestHandler<GetManufacturerCreditTotalsQuery, ActionResult<List<PeriodCreditTotals>>>> logger,
        IInstallationCreditRepository creditLedgerRepository,
        ILicenceHolderService licenceHolderService,
        IRequestValidator requestValidator,
        IMcsSynchronisationService mcsSynchronisationService) : base(logger)
    {
        _creditLedgerRepository = creditLedgerRepository;
        _licenceHolderService = licenceHolderService;
        _requestValidator = requestValidator;
        _mcsSynchronisationService = mcsSynchronisationService;
    }

    public override async Task<ActionResult<List<PeriodCreditTotals>>> Handle(GetManufacturerCreditTotalsQuery query, CancellationToken cancellationToken)
    {
        var organisationId = query.OrganisationId;
        var schemeYearId = query.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId,
            requireActiveOrganisation: true,
            schemeYearId: schemeYearId);
        if (validationError != null)
            return validationError;

        var installationRequestsResponse = await _mcsSynchronisationService.GetInstallationRequests(schemeYearId);
        if (!installationRequestsResponse.IsSuccessStatusCode || installationRequestsResponse.Result == null)
            return CannotGetManufacturerInstallationRequests(schemeYearId);
        var installationRequests = installationRequestsResponse.Result;

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

        var periods = GetCreditTotals(installationCredits, installationRequests);

        return periods.ToList();
    }

    protected IEnumerable<PeriodCreditTotals> GetCreditTotals(IList<InstallationCredit> installationCredits, List<InstallationRequestSummaryDto> installationRequests)
    {
        return installationRequests
            .Select(x => new { StartDate = DateOnly.FromDateTime(x.StartDate), EndDate = DateOnly.FromDateTime(x.EndDate) })
            .Select(interval =>
            {

                return new PeriodCreditTotals(interval.StartDate,
                                               interval.EndDate,
                                               installationCredits.Where(x => (x.DateCreditGenerated >= interval.StartDate && x.DateCreditGenerated <= interval.EndDate) && x.IsHybrid == false).Count(),
                                               installationCredits.Where(x => (x.DateCreditGenerated >= interval.StartDate && x.DateCreditGenerated <= interval.EndDate) && x.IsHybrid == true).Count(),
                                               installationCredits.Where(x => (x.DateCreditGenerated >= interval.StartDate && x.DateCreditGenerated <= interval.EndDate) && x.IsHybrid == false).Sum(x => x.Value),
                                               installationCredits.Where(x => (x.DateCreditGenerated >= interval.StartDate && x.DateCreditGenerated <= interval.EndDate) && x.IsHybrid == true).Sum(x => x.Value));
            });
    }
}