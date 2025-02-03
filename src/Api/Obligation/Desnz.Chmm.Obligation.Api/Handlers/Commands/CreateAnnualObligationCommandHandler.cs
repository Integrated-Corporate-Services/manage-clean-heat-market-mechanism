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
    public class CreateAnnualObligationCommandHandler : BaseRequestHandler<CreateAnnualObligationCommand, ActionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserService _userService;
        private readonly IObligationCalculator _obligationCalculator;
        private readonly ISchemeYearService _schemeYearService;
        private readonly IRequestValidator _requestValidator;

        public CreateAnnualObligationCommandHandler(
            ILogger<BaseRequestHandler<CreateAnnualObligationCommand, ActionResult>> logger,
            ITransactionRepository transactionRepository,
            ICurrentUserService userService,
            IObligationCalculator obligationCalculator,
            ISchemeYearService schemeYearService,
            IRequestValidator requestValidator) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _userService = userService;
            _obligationCalculator = obligationCalculator;
            _schemeYearService = schemeYearService;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult> Handle(CreateAnnualObligationCommand command, CancellationToken cancellationToken)
        {
            var organisationId = command.OrganisationId;
            var schemeYearId = command.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
                organisationId: organisationId,
                requireActiveOrganisation: true,
                schemeYearId: schemeYearId);
            if (validationError != null)
                return validationError;

            var existingTransactions = await _transactionRepository.GetAll(x => x.SchemeYearId == command.SchemeYearId &&
                                                                                x.OrganisationId == command.OrganisationId &&
                                                                                x.TransactionType == TransactionType.AnnualSubmission);

            if (existingTransactions.Any())
            {
                if (!(command.Override && _userService.CurrentUser.IsAdmin()))
                {
                    return ObligationAlreadySumbitted(command.SchemeYearId, command.OrganisationId);
                }
                await _transactionRepository.Remove(existingTransactions, true);
            }

            var obligationCalculationsResponse = await _schemeYearService.GetObligationCalculations(schemeYearId, cancellationToken);
            if (!obligationCalculationsResponse.IsSuccessStatusCode || obligationCalculationsResponse.Result == null)
                return CannotLoadObligationCalculations(schemeYearId, obligationCalculationsResponse.Problem);
            var obligationCalculations = obligationCalculationsResponse.Result;

            int obligation = _obligationCalculator.Calculate(command.Gas.Value, command.Oil.Value, obligationCalculations.GasBoilerSalesThreshold, obligationCalculations.OilBoilerSalesThreshold, obligationCalculations.TargetRate);

            var quarterlyTransactions = await _transactionRepository.GetAll(x => x.SchemeYearId == schemeYearId &&
                                                                                 x.TransactionType == TransactionType.QuarterlySubmission, withTracking: true);
            quarterlyTransactions.ForEach(x => x.Exclude());

            var userId = _userService.CurrentUser.GetUserId();
            var annualTransaction = new Entities.Transaction(userId.Value,
                                                           TransactionType.AnnualSubmission,
                                                           command.OrganisationId,
                                                           command.SchemeYearId,
                                                           obligation,
                                                           command.TransactionDate);

            var transactionId = await _transactionRepository.Create(annualTransaction);
            await _transactionRepository.UnitOfWork.SaveChangesAsync();

            return Responses.Created(transactionId);
        }
    }
}
