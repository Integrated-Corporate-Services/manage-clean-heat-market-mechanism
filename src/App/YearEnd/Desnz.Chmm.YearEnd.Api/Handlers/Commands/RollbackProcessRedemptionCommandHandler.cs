using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.YearEnd.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.YearEnd.Api.Handlers.Commands
{
    public class RollbackProcessRedemptionCommandHandler : BaseRequestHandler<RollbackRedemptionCommand, ActionResult>
    {
        private readonly ICreditLedgerService _creditLedgerService;
        private readonly IObligationService _obligationService;
        private readonly ISchemeYearService _schemeYearService;

        public RollbackProcessRedemptionCommandHandler(
            ILogger<BaseRequestHandler<RollbackRedemptionCommand, ActionResult>> logger,
            ICreditLedgerService creditLedgerService,
            IObligationService obligationService,
            ISchemeYearService schemeYearService) : base(logger)
        {
            _creditLedgerService = creditLedgerService;
            _obligationService = obligationService;
            _schemeYearService = schemeYearService;
        }

        public override async Task<ActionResult> Handle(RollbackRedemptionCommand command, CancellationToken cancellationToken)
        {
            var schemeYearId = command.SchemeYearId;

            var httpResponseSchemeYear = await _schemeYearService.GetSchemeYear(schemeYearId, cancellationToken);
            if (!httpResponseSchemeYear.IsSuccessStatusCode || httpResponseSchemeYear.Result == null)
                return CannotLoadSchemeYear(schemeYearId, httpResponseSchemeYear.Problem);

            var tasks = new List<Task<HttpObjectResponse<object>>>
            {
                _obligationService.RollbackRedeemObligations(schemeYearId),
                _creditLedgerService.RollbackRedeemCredits(schemeYearId)
            };

            await Task.WhenAll(tasks);

            foreach (var taskResponses in tasks)
            {
                var httpResponse = taskResponses.Result;
                if (!httpResponse.IsSuccessStatusCode)
                    return CannotRollbackRedemptionProcess(httpResponse.Problem);
            }

            return Responses.Ok();
        }
    }
}
