using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;
using Desnz.Chmm.Obligation.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Api.Handlers.Commands
{
    public class RollbackCarryForwardObligationCommandHandler : BaseRequestHandler<RollbackCarryForwardObligationCommand, ActionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISchemeYearService _schemeYearService;
        private readonly IRequestValidator _requestValidator;

        public RollbackCarryForwardObligationCommandHandler(
            ILogger<BaseRequestHandler<RollbackCarryForwardObligationCommand, ActionResult>> logger,
            ITransactionRepository transactionRepository,
            ISchemeYearService schemeYearService,
            IRequestValidator requestValidator) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _schemeYearService = schemeYearService;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult> Handle(RollbackCarryForwardObligationCommand command, CancellationToken cancellationToken)
        {
            var schemeYearId = command.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(schemeYearId: schemeYearId);
            if (validationError != null)
                return validationError;

            var httpResponseNextSchemeYear = await _schemeYearService.GetNextSchemeYear(schemeYearId, cancellationToken);
            if (httpResponseNextSchemeYear == null || !httpResponseNextSchemeYear.IsSuccessStatusCode)
                return CannotLoadNextSchemeYear(schemeYearId, httpResponseNextSchemeYear.Problem);

            await _transactionRepository.RollbackCarryForwardObligation(command.SchemeYearId, httpResponseNextSchemeYear.Result.Id);

            return Responses.Ok();
        }
    }
}
