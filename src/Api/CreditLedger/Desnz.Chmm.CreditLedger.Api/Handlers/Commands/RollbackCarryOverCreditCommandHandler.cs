using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Commands
{
    public class RollbackCarryOverCreditCommandHandler : BaseRequestHandler<RollbackCarryOverCreditCommand, ActionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISchemeYearService _schemeYearService;
        private readonly IRequestValidator _requestValidator;

        public RollbackCarryOverCreditCommandHandler(
            ILogger<BaseRequestHandler<RollbackCarryOverCreditCommand, ActionResult>> logger,
            ITransactionRepository transactionRepository,
            ISchemeYearService schemeYearService,
            IRequestValidator requestValidator) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _schemeYearService = schemeYearService;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult> Handle(RollbackCarryOverCreditCommand command, CancellationToken cancellationToken)
        {
            var schemeYearId = command.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(schemeYearId: schemeYearId);
            if (validationError != null)
                return validationError;

            var httpResponseNextSchemeYear = await _schemeYearService.GetNextSchemeYear(schemeYearId, cancellationToken);
            if (httpResponseNextSchemeYear == null || !httpResponseNextSchemeYear.IsSuccessStatusCode)
                return CannotLoadNextSchemeYear(schemeYearId, httpResponseNextSchemeYear.Problem);

            await _transactionRepository.RollbackCarryForwardCredit(command.SchemeYearId, httpResponseNextSchemeYear.Result.Id);

            return Responses.Ok();
        }
    }
}
