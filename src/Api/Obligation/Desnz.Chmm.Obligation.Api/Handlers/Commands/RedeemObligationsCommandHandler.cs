using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Obligation.Api.Constants;
using Desnz.Chmm.Obligation.Api.Entities;
using Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;
using Desnz.Chmm.Obligation.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Api.Handlers.Commands
{
    public class RedeemObligationsCommandHandler : BaseRequestHandler<RedeemObligationsCommand, ActionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IRequestValidator _requestValidator;

        public RedeemObligationsCommandHandler(
            ILogger<BaseRequestHandler<RedeemObligationsCommand, ActionResult>> logger,
            ITransactionRepository transactionRepository,
            IDateTimeProvider dateTimeProvider,
            IRequestValidator requestValidator) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _dateTimeProvider = dateTimeProvider;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult> Handle(RedeemObligationsCommand command, CancellationToken cancellationToken)
        {
            var schemeYearId = command.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(schemeYearId: schemeYearId);
            if (validationError != null)
                return validationError;

            var transactions = new List<Transaction>();

            foreach(var transaction in command.Redemptions)
            {
                transactions.Add(new Transaction(
                    IdentityConstants.System.SystemUserId,
                    TransactionConstants.TransactionType.Redeemed,
                    transaction.OrganisationId,
                    schemeYearId,
                    transaction.Value,
                    _dateTimeProvider.UtcNow));
            }

            await _transactionRepository.AddTransactions(transactions);

            return Responses.Ok();
        }
    }
}
