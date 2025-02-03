using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Obligation.Api.Constants;
using Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;
using Desnz.Chmm.Obligation.Api.Services;
using Desnz.Chmm.Obligation.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Api.Handlers.Commands
{
    public class AmendObligationCommandHandler : BaseRequestHandler<AmendObligationCommand, ActionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserService _userService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IRequestValidator _requestValidator;
        private readonly IObligationCalculator _calculator;
        private readonly IValidationMessenger _validationMessenger;

        public AmendObligationCommandHandler(
            ILogger<BaseRequestHandler<AmendObligationCommand, ActionResult>> logger,
            ITransactionRepository transactionRepository, 
            ICurrentUserService userService, 
            IDateTimeProvider dateTimeProvider,
            IRequestValidator requestValidator,
            IObligationCalculator calculator,
            IValidationMessenger validationMessenger) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _userService = userService;
            _dateTimeProvider = dateTimeProvider;
            _requestValidator = requestValidator;
            _calculator = calculator;
            _validationMessenger = validationMessenger;
        }

        public override async Task<ActionResult> Handle(AmendObligationCommand command, CancellationToken cancellationToken)
        {
            var organisationId = command.OrganisationId;
            var schemeYearId = command.SchemeYearId;
            var now = _dateTimeProvider.UtcDateNow;

            var validation = new CustomValidator<SchemeYearDto>
            {
                ValidationFunction = i => now < i.StartDate || now >= i.SurrenderDayDate,
                FailureAction = i => { return CannotAdjustObligationsAfterEndOfSchemeYear(i.Id); }
            };

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
                organisationId: organisationId,
                requireActiveOrganisation: true,
                schemeYearId: schemeYearId,
                schemeYearValidation: validation);
            if (validationError != null)
                return validationError;

            //Get the transactions for the org to work out the current balance
            var transactions = await _transactionRepository.GetAll(t =>
                t.SchemeYearId == schemeYearId &&
                t.OrganisationId == organisationId &&
                t.IsExcluded == false,
                RepositoryConstants.SortOrder.Ascending,
                false);

            var currentObligationBalance = _calculator.CalculateCurrentObligationBalance(transactions);

            if((currentObligationBalance + command.Value) < 0)
            {
                return _validationMessenger.InvalidObligationAmendment(organisationId);
            }

            var userId = _userService.CurrentUser.GetUserId();

            var transaction = new Entities.Transaction(userId.Value, TransactionConstants.TransactionType.AdminAdjustment, organisationId, schemeYearId, command.Value, _dateTimeProvider.UtcNow);
            var transactionId = await _transactionRepository.Create(transaction);
            return Responses.Created(transactionId);
        }
    }
}
