using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Commands
{
    public class RollbackRedeemCreditsCommandHandler : BaseRequestHandler<RollbackRedeemCreditsCommand, ActionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IRequestValidator _requestValidator;

        public RollbackRedeemCreditsCommandHandler(
            ILogger<BaseRequestHandler<RollbackRedeemCreditsCommand, ActionResult>> logger,
            ITransactionRepository transactionRepository,
            IRequestValidator requestValidator) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult> Handle(RollbackRedeemCreditsCommand command, CancellationToken cancellationToken)
        {
            var schemeYearId = command.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(schemeYearId: schemeYearId);
            if (validationError != null)
                return validationError;

            await _transactionRepository.RollbackRedemptionTransactions(command.SchemeYearId);

            return Responses.Ok();
        }
    }
}
