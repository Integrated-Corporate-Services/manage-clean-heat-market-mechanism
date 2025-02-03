using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;
using Desnz.Chmm.Obligation.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Api.Handlers.Commands
{
    public class RollbackRedeemObligationsCommandHandler : BaseRequestHandler<RollbackRedeemObligationsCommand, ActionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IRequestValidator _requestValidator;

        public RollbackRedeemObligationsCommandHandler(
            ILogger<BaseRequestHandler<RollbackRedeemObligationsCommand, ActionResult>> logger,
            ITransactionRepository transactionRepository,
            IRequestValidator requestValidator) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult> Handle(RollbackRedeemObligationsCommand command, CancellationToken cancellationToken)
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
