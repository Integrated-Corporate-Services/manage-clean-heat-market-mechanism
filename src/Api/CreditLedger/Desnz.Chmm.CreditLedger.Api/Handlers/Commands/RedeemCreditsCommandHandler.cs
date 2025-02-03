using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.CreditLedger.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Commands
{
    public class RedeemCreditsCommandHandler : BaseRequestHandler<RedeemCreditsCommand, ActionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IRequestValidator _requestValidator;

        public RedeemCreditsCommandHandler(
            ILogger<BaseRequestHandler<RedeemCreditsCommand, ActionResult>> logger,
            ITransactionRepository transactionRepository,
            IDateTimeProvider dateTimeProvider,
        IRequestValidator requestValidator) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _dateTimeProvider = dateTimeProvider;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult> Handle(RedeemCreditsCommand command, CancellationToken cancellationToken)
        {
            var schemeYearId = command.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(schemeYearId: schemeYearId);
            if (validationError != null)
                return validationError;

            var transactions = new List<Transaction>();

            foreach(var transaction in command.Redemptions)
            {
                transactions.Add(new Transaction(
                    schemeYearId,
                    transaction.OrganisationId,
                    transaction.Value,
                    IdentityConstants.System.SystemUserId,
                    CreditLedgerConstants.TransactionType.Redeemed,
                    _dateTimeProvider.UtcNow));
            }

            await _transactionRepository.AddTransactions(transactions);

            return Responses.Ok();
        }
    }
}
