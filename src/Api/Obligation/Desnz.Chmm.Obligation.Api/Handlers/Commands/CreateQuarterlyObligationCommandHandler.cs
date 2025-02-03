using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;
using Desnz.Chmm.Obligation.Api.Services;
using Desnz.Chmm.Obligation.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Obligation.Api.Constants.TransactionConstants;

namespace Desnz.Chmm.Obligation.Api.Handlers.Commands
{
    public class CreateQuarterlyObligationCommandHandler : BaseRequestHandler<CreateQuarterlyObligationCommand, ActionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserService _userService;
        private readonly IBoilerSalesService _boilerSalesService;
        private readonly IObligationCalculator _obligationCalculator;
        private readonly ISchemeYearService _schemeYearService;
        private readonly IRequestValidator _requestValidator;

        public CreateQuarterlyObligationCommandHandler(
            ILogger<BaseRequestHandler<CreateQuarterlyObligationCommand, ActionResult>> logger,
            ITransactionRepository transactionRepository, 
            ICurrentUserService userService, 
            IBoilerSalesService boilerSalesService,
            IObligationCalculator obligationCalculator,
            ISchemeYearService schemeYearService,
            IRequestValidator requestValidator) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _userService = userService;
            _boilerSalesService = boilerSalesService;
            _obligationCalculator = obligationCalculator;
            _schemeYearService = schemeYearService;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult> Handle(CreateQuarterlyObligationCommand command, CancellationToken cancellationToken)
        {
            var organisationId = command.OrganisationId;
            var schemeYearId = command.SchemeYearId;
            var quarterId = command.SchemeYearQuarterId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
                organisationId: organisationId,
                requireActiveOrganisation: true,
                schemeYearId: schemeYearId,
                schemeYearQuarterId: quarterId);
            if (validationError != null)
                return validationError;

            var existingTransaction = await _transactionRepository.GetQuarterTransaction(organisationId, schemeYearId, quarterId);

            if (existingTransaction != null)
            {
                if (!(command.Override && _userService.CurrentUser.IsAdmin()))
                {
                    return ObligationAlreadySumbitted(command.SchemeYearId, command.OrganisationId);
                }
                await _transactionRepository.Remove(new List<Entities.Transaction> { existingTransaction }, true);
            }

            var quarterlyBoilerSalesResponse = await _boilerSalesService.GetQuarterlyBoilerSales(organisationId, schemeYearId);
            if (!quarterlyBoilerSalesResponse.IsSuccessStatusCode || quarterlyBoilerSalesResponse.Result == null)
                return CannotLoadBoilerSalesData(schemeYearId, quarterId, organisationId, quarterlyBoilerSalesResponse.Problem);
            var quarterlyBoilerSales = quarterlyBoilerSalesResponse.Result;

            var quarterlyTransactionsToDate = await _transactionRepository.GetQuarterTransactions(organisationId, schemeYearId);

            var obligationCalculationsResponse = await _schemeYearService.GetObligationCalculations(schemeYearId, cancellationToken);
            if (!obligationCalculationsResponse.IsSuccessStatusCode)
                return CannotLoadObligationCalculations(schemeYearId);
            var obligationCalculations = obligationCalculationsResponse.Result;

            int obligation = _obligationCalculator.Calculate(command.Gas.Value, command.Oil.Value, quarterlyBoilerSales, quarterlyTransactionsToDate.Select(x => x.Obligation), obligationCalculations.GasBoilerSalesThreshold, obligationCalculations.OilBoilerSalesThreshold, obligationCalculations.TargetRate);

            var userId = _userService.CurrentUser.GetUserId();
            var transaction = new Entities.Transaction(userId.Value,
                                                       TransactionType.QuarterlySubmission,
                                                       organisationId,
                                                       schemeYearId,
                                                       obligation,
                                                       command.TransactionDate,
                                                       quarterId);

            // If we've already got an annual transaction, this should be excluded.
            var annualTransaction = await _transactionRepository.GetAnnualTransaction(organisationId, schemeYearId);
            if (annualTransaction != null)
            {
                transaction.Exclude();
            }

            var transactionId = await _transactionRepository.Create(transaction);
            return Responses.Created(transactionId);
        }
    }
}
