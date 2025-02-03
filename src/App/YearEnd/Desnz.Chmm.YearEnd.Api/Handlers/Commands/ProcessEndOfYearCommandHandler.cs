using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Configuration.Common.Commands;
using Desnz.Chmm.YearEnd.Api.Controllers;
using Desnz.Chmm.YearEnd.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Desnz.Chmm.YearEnd.Api.Handlers.Commands;

public class ProcessEndOfYearCommandHandler : ProcessEndOfYearBase<ProcessEndOfYearCommand, ActionResult>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public ProcessEndOfYearCommandHandler(
        ILogger<BaseRequestHandler<ProcessEndOfYearCommand, ActionResult>> logger,
        ICreditLedgerService creditLedgerService,
        IObligationService obligationService,
        ISchemeYearService schemeYearService,
        IIdentityService identityService,
        IYearEndService endOfYearService,
        IOptions<ApiKeyPolicyConfig> apiKeyPolicyConfig,
        IHttpContextAccessor httpContextAccessor) : base(logger,
        creditLedgerService,
        obligationService,
        schemeYearService,
        identityService,
        endOfYearService,
        apiKeyPolicyConfig,
        httpContextAccessor)
    {
    }

    public override async Task<ActionResult> Handle(ProcessEndOfYearCommand command, CancellationToken cancellationToken)
    {
        var actionResult = await GetIdentityToken();
        if (actionResult.Result is BadRequestObjectResult)
            return actionResult.Result;
        var identityToken = actionResult.Value;

        var httpResponseSchemeYear = await SchemeYearService.GetCurrentSchemeYearBySurrenderDay(cancellationToken, identityToken);
        if (!httpResponseSchemeYear.IsSuccessStatusCode || httpResponseSchemeYear.Result == null)
            return CannotLoadCurrentSchemeYear(httpResponseSchemeYear.Problem);
        var schemeYearId = httpResponseSchemeYear.Result.Id;

        var httpResponseNextSchemeYear = await SchemeYearService.GetNextSchemeYear(schemeYearId, cancellationToken, identityToken);
        if (!httpResponseNextSchemeYear.IsSuccessStatusCode || httpResponseNextSchemeYear.Result == null)
            return CannotLoadNextSchemeYear(schemeYearId, httpResponseNextSchemeYear.Problem);
        var nextSchemeYearId = httpResponseNextSchemeYear.Result.Id;

        var httpRedemptionResponse = await EndOfYearService.ProcessRedemption(new ProcessRedemptionCommand(schemeYearId), identityToken);
        if (!httpRedemptionResponse.IsSuccessStatusCode)
        {
            return EndOfYearProcessFailed(httpRedemptionResponse.Problem);
        }

        var tasks = new List<Task<HttpObjectResponse<object>>>
        {
            CreditLedgerService.CarryOverCredit(schemeYearId, identityToken),
            ObligationService.CarryForwardObligation(schemeYearId, identityToken),
            SchemeYearService.GenerateNextYearsSchemea(new GenerateNextYearsSchemeaCommand(schemeYearId), cancellationToken, identityToken)
        };
        await Task.WhenAll(tasks);

        if(tasks.All(x => x.IsCompletedSuccessfully && x.Result.IsSuccessStatusCode))
            return Responses.Ok();

        var rollbackResult = await RollbackEndOfYear(schemeYearId, nextSchemeYearId, cancellationToken, identityToken);
        return rollbackResult is OkResult?
            EndOfYearProcessFailed(tasks.First(x => !x.IsCompletedSuccessfully || !x.Result.IsSuccessStatusCode).Result.Problem):
            rollbackResult;
    }
}
