using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Obligation.Api.Entities;
using Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;
using Desnz.Chmm.Obligation.Api.Services;
using Desnz.Chmm.Obligation.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;
using static Desnz.Chmm.Obligation.Api.Constants.TransactionConstants;

namespace Desnz.Chmm.Obligation.Api.Handlers.Commands
{
    /// <summary>
    /// Handles carry-forward obligation calculations for all organisations for the specified scheme year
    /// </summary>
    public class CarryForwardObligationCommandHandler : BaseRequestHandler<CarryForwardObligationCommand, ActionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrganisationService _organisationService;
        private readonly ILogger<BaseRequestHandler<CarryForwardObligationCommand, ActionResult>> _logger;
        private readonly IBoilerSalesService _boilerSalesService;
        private readonly ICreditLedgerService _creditLedgerService;
        private readonly ICarryForwardObligationCalculator _carryForwardObligationCalculator;
        private readonly ISchemeYearService _schemeYearService;
        private readonly IDateTimeProvider _dateTimeProvider;

        /// <summary>
        /// Initializes the handler
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="organisationService"></param>
        /// <param name="boilerSalesService"></param>
        /// <param name="creditLedgerService"></param>
        /// <param name="schemeYearService"></param>
        /// <param name="carryForwardObligationCalculator"></param>
        /// <param name="transactionRepository"></param>
        /// <param name="dateTimeProvider"></param>
        public CarryForwardObligationCommandHandler(
            ILogger<BaseRequestHandler<CarryForwardObligationCommand, ActionResult>> logger,
            IOrganisationService organisationService,
            IBoilerSalesService boilerSalesService,
            ICreditLedgerService creditLedgerService,
            ISchemeYearService schemeYearService,
            ICarryForwardObligationCalculator carryForwardObligationCalculator,
            ITransactionRepository transactionRepository,
            IDateTimeProvider dateTimeProvider) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _organisationService = organisationService;
            _logger = logger;
            _boilerSalesService = boilerSalesService;
            _creditLedgerService = creditLedgerService;
            _carryForwardObligationCalculator = carryForwardObligationCalculator;
            _schemeYearService = schemeYearService;
            _dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Handles carry-forward obligation calculations for all organisations for the specified scheme year
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<ActionResult> Handle(CarryForwardObligationCommand command, CancellationToken cancellationToken)
        {
            var schemeYearsActionResult = await GetCurrentAndNextSchemeYears(command.SchemeYearId, cancellationToken);
            if (schemeYearsActionResult.Result is BadRequestObjectResult)
                return schemeYearsActionResult.Result;
            (Guid CurrentSchemeYearId, Guid NextSchemeYearId) schemeYears = schemeYearsActionResult.Value;

            var organisationsResponse = await _organisationService.GetManufacturers();
            if (organisationsResponse.Result == null || !organisationsResponse.IsSuccessStatusCode)
                return CannotLoadOrganisations(organisationsResponse.Problem);

            var organisations = organisationsResponse.Result.Where(x => x.Status == OrganisationConstants.Status.Active);
            if (!organisations.Any())
                return Responses.Ok();

            var obligationCalculationsConfigurationResponse = await _schemeYearService.GetObligationCalculations(command.SchemeYearId, cancellationToken);
            if (!obligationCalculationsConfigurationResponse.IsSuccessStatusCode || obligationCalculationsConfigurationResponse.Result == null)
                return CannotLoadObligationCalculations(command.SchemeYearId, obligationCalculationsConfigurationResponse.Problem);
            var obligationCalculationsConfiguration = obligationCalculationsConfigurationResponse.Result;

            var boilerSalesSummaryResponse = await _boilerSalesService.GetAllBoilerSalesSummary(command.SchemeYearId);
            if (!boilerSalesSummaryResponse.IsSuccessStatusCode || boilerSalesSummaryResponse.Result == null)
                return CannotLoadBoilerSalesSummaries(command.SchemeYearId, boilerSalesSummaryResponse.Problem);

            var allBoilerSalesSummary = boilerSalesSummaryResponse.Result;
            if (!allBoilerSalesSummary.Any())
                return NoBoilerSalesSummaries(command.SchemeYearId);

            if (allBoilerSalesSummary.Any(x => x.BoilerSalesSubmissionStatus != BoilerSalesSummarySubmissionStatus.AnnualApproved))
                return UnapprovedBoilerSales(command.SchemeYearId);

            var creditTotalsResponse = await _creditLedgerService.GetAllCreditBalances(command.SchemeYearId);
            if (creditTotalsResponse.Result == null || !creditTotalsResponse.IsSuccessStatusCode)
                return CannotLoadCreditBalances(command.SchemeYearId, creditTotalsResponse.Problem);

            var allCreditTotals = creditTotalsResponse.Result;
            if (!allCreditTotals.Any())
                return NoCreditBalances(command.SchemeYearId);

            var allObligationTransactions = await _transactionRepository.GetAll(x => x.SchemeYearId == command.SchemeYearId &&
                                                                                    (x.TransactionType == TransactionType.AnnualSubmission ||
                                                                                    x.TransactionType == TransactionType.AdminAdjustment ||
                                                                                    x.TransactionType == TransactionType.BroughtFowardFromPreviousYear));
            if (allObligationTransactions == null)
                return CannotLoadAnnualObligation(command.SchemeYearId);


            var allExistingTransactions = await _transactionRepository.GetAll(x => x.SchemeYearId == schemeYears.CurrentSchemeYearId &&
                                                                                   x.TransactionType == TransactionType.CarryForwardToNextYear &&
                                                                                   organisations.Select(x => x.Id).Contains(x.OrganisationId) ||
                                                                                   x.SchemeYearId == schemeYears.NextSchemeYearId &&
                                                                                   x.TransactionType == TransactionType.BroughtFowardFromPreviousYear &&
                                                                                   organisations.Select(x => x.Id).Contains(x.OrganisationId));
            if (allExistingTransactions != null && allExistingTransactions.Any())
                return CarryForwardObligationProcessAlreadyRun(command.SchemeYearId);

            var newTransactions = new ConcurrentBag<Transaction>();

            foreach (var organisation in organisations)
            {
                SingleOrganisationRun(organisation.Id,
                                      schemeYears.CurrentSchemeYearId,
                                      schemeYears.NextSchemeYearId,
                                      obligationCalculationsConfiguration,
                                      allBoilerSalesSummary.FirstOrDefault(x => x.OrganisationId == organisation.Id),
                                      allCreditTotals.FirstOrDefault(x => x.OrganisationId == organisation.Id),
                                      allObligationTransactions.Where(x => x.OrganisationId == organisation.Id).ToList(),
                                      newTransactions);
            }

            await _transactionRepository.AddTransactions(newTransactions.ToList());

            return Responses.Ok();
        }

        private void SingleOrganisationRun(Guid organisationId,
                                           Guid currentSchemeYearId, 
                                           Guid nextSchemeYearId,
                                           ObligationCalculationsDto obligationCalculationsConfiguration,
                                           BoilerSalesSummaryDto? boilerSalesSummary,
                                           OrganisationCreditBalanceDto? creditTotals,
                                           List<Transaction> obligationTransactions,
                                           ConcurrentBag<Transaction> newTransactions)
        {
            if (boilerSalesSummary == null || obligationTransactions == null || creditTotals == null)
            {
                _logger.LogWarning($"Insufficient data to calculate the Carry Forward Obligation for Organisation Id: {organisationId}");
                return;
            }

            var creditsCap = boilerSalesSummary.GasBoilerSales == 0 ? obligationCalculationsConfiguration.OilCreditsCap : obligationCalculationsConfiguration.GasCreditsCap;

            int carryForwardObligation = _carryForwardObligationCalculator.Calculate(obligationTransactions,
                                                                                     creditTotals.CreditBalance,
                                                                                     creditsCap,
                                                                                     obligationCalculationsConfiguration.PercentageCap,
                                                                                     obligationCalculationsConfiguration.TargetMultiplier);

            newTransactions.Add(new Transaction(IdentityConstants.System.SystemUserId,
                                                TransactionType.CarryForwardToNextYear,
                                                organisationId,
                                                currentSchemeYearId,
                                                -carryForwardObligation,
                                                _dateTimeProvider.UtcNow));
            newTransactions.Add(new Transaction(IdentityConstants.System.SystemUserId,
                                                TransactionType.BroughtFowardFromPreviousYear,
                                                organisationId,
                                                nextSchemeYearId,
                                                carryForwardObligation,
                                                _dateTimeProvider.UtcNow));
        }

        private async Task<ActionResult<(Guid, Guid)>> GetCurrentAndNextSchemeYears(Guid schemeYearId, CancellationToken cancellationToken)
        {
            var allSchemeYearsResponse = await _schemeYearService.GetAllSchemeYears(cancellationToken);
            if (!allSchemeYearsResponse.IsSuccessStatusCode || allSchemeYearsResponse.Result == null)
                return CannotLoadAllSchemeYears(allSchemeYearsResponse.Problem);
            var allSchemeYears = allSchemeYearsResponse.Result;

            var currentSchemeYear = allSchemeYears.FirstOrDefault(x => x.Id == schemeYearId);
            if (currentSchemeYear == null)
                return CannotLoadSchemeYear(schemeYearId);

            var nextSchemeYear = allSchemeYears.FirstOrDefault(x => x.PreviousSchemeYearId == currentSchemeYear.Id);
            if (nextSchemeYear == null)
                return CannotLoadNextSchemeYear(currentSchemeYear.Id);

            return (currentSchemeYear.Id, nextSchemeYear.Id);
        }
    }
}
