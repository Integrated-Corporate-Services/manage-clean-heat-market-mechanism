using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.Obligation.Common.Dtos;
using Desnz.Chmm.YearEnd.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Desnz.Chmm.YearEnd.Api.Handlers.Commands;

public class ProcessRedemptionCommandHandler : BaseRequestHandler<ProcessRedemptionCommand, ActionResult>
{
    private readonly ICreditLedgerService _creditLedgerService;
    private readonly IObligationService _obligationService;
    private readonly IOrganisationService _organisationService;
    private readonly ILicenceHolderService _licenceHolderService;
    private readonly ISchemeYearService _schemeYearService;

    /// <summary>
    /// Default constructor
    /// </summary>

    public ProcessRedemptionCommandHandler(
        ILogger<BaseRequestHandler<ProcessRedemptionCommand, ActionResult>> logger,
        ICreditLedgerService creditLedgerService,
        IObligationService obligationService,
        IOrganisationService organisationService,
        ILicenceHolderService licenceHolderService,
        ISchemeYearService schemeYearService) : base(logger)
    {
        _creditLedgerService = creditLedgerService;
        _obligationService = obligationService;
        _organisationService = organisationService;
        _licenceHolderService = licenceHolderService;
        _schemeYearService = schemeYearService;
    }

    public override async Task<ActionResult> Handle(ProcessRedemptionCommand command, CancellationToken cancellationToken)
    {
        var schemeYearId = command.SchemeYearId;

        var httpResponseSchemeYear = await _schemeYearService.GetSchemeYear(schemeYearId, cancellationToken);
        if (!httpResponseSchemeYear.IsSuccessStatusCode || httpResponseSchemeYear.Result == null)
            return CannotLoadSchemeYear(schemeYearId, httpResponseSchemeYear.Problem);

        var licenceHoldersResponse = await _licenceHolderService.GetAll();
        if (!licenceHoldersResponse.IsSuccessStatusCode || licenceHoldersResponse.Result == null)
            return CannotLoadLicenceHolders(licenceHoldersResponse.Problem);

        var organisations = await _organisationService.GetActiveManufacturers();
        if (!organisations.IsSuccessStatusCode || organisations.Result == null)
            return CannotLoadOrganisations(organisations.Problem);

        var obligationTotals = await _obligationService.GetSchemeYearObligationTotals(schemeYearId);
        if (!obligationTotals.IsSuccessStatusCode || obligationTotals.Result == null)
            return CannotLoadObligationTotals(schemeYearId, obligationTotals.Problem);

        var creditTotals = await _creditLedgerService.GetAllCreditBalances(schemeYearId);
        if (!creditTotals.IsSuccessStatusCode || creditTotals.Result == null)
            return CannotLoadCreditBalances(schemeYearId, creditTotals.Problem);

        var creditRedemptions = new List<CreditRedemptionDto>();
        var obligationRedemptions = new List<ObligationRedemptionDto>();

        foreach (var organisation in organisations.Result)
        {
            var obligationTotal = obligationTotals.Result.SingleOrDefault(i => i.OrganisationId == organisation.Id);
            var creditTotal = creditTotals.Result.SingleOrDefault(i => i.OrganisationId == organisation.Id);

            if (obligationTotal == null || creditTotal == null)
                return CannotFindCreditOrObligationTotal(organisation.Id);

            var totalCredits = creditTotal.CreditBalance;
            var totalObligations = obligationTotal.Value;

            // We don't need to redeem anything, if 0...
            if (totalCredits == 0 && totalObligations == 0) continue;

            var redeemed = Math.Min(totalObligations, totalCredits);

            obligationRedemptions.Add(new ObligationRedemptionDto(organisation.Id, -redeemed));
            creditRedemptions.Add(new CreditRedemptionDto(organisation.Id, -redeemed));
        }

        var obligationLog = new RedeemCreditsCommand(schemeYearId, obligationRedemptions);
        var creditLog = new RedeemObligationsCommand(schemeYearId, creditRedemptions);

        var obligationResponse = await _obligationService.RedeemObligations(creditLog);

        if (!obligationResponse.IsSuccessStatusCode)
            return CannotLogObligationRedemption(schemeYearId, obligationResponse.Problem);

        var creditResponse = await _creditLedgerService.RedeemCredits(obligationLog);
        if (!creditResponse.IsSuccessStatusCode)
        {
            var rollbackResponse = await _obligationService.RollbackRedeemObligations(schemeYearId);
            if(!rollbackResponse.IsSuccessStatusCode)
                return CannotLogCreditRedemption(schemeYearId, creditResponse.Problem, rollbackResponse.Problem);

            return CannotLogCreditRedemption(schemeYearId, creditResponse.Problem);
        }

        return Responses.Ok();
    }
}