using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.YearEnd.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Desnz.Chmm.YearEnd.Api.Handlers.Commands;

/// <summary>
/// Rolls back the End-of-Year process changes
/// </summary>
public class RollbackEndOfYearCommandHandler : ProcessEndOfYearBase<RollbackEndOfYearCommand, ActionResult>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public RollbackEndOfYearCommandHandler(
        ILogger<BaseRequestHandler<RollbackEndOfYearCommand, ActionResult>> logger,
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

    /// <summary>
    /// Handles the End-of-Year process changes' rolling back 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<ActionResult> Handle(RollbackEndOfYearCommand command, CancellationToken cancellationToken)
    {
        var actionResult = await GetIdentityToken();
        if (actionResult.Result is BadRequestObjectResult)
            return actionResult.Result;
        var identityToken = actionResult.Value;

        var currentSchemeYearsResponse = await SchemeYearService.GetSchemeYear(command.SchemeYearId, cancellationToken, identityToken);
        if (!currentSchemeYearsResponse.IsSuccessStatusCode || currentSchemeYearsResponse.Result == null)
            return CannotLoadAllSchemeYears(currentSchemeYearsResponse.Problem);

        var nextSchemeYearsResponse = await SchemeYearService.GetNextSchemeYear(command.SchemeYearId, cancellationToken, identityToken);
        if (!nextSchemeYearsResponse.IsSuccessStatusCode || nextSchemeYearsResponse.Result == null)
            return CannotLoadNextSchemeYear(command.SchemeYearId, nextSchemeYearsResponse.Problem);

        var nextSchemeYear = nextSchemeYearsResponse.Result;

        return await RollbackEndOfYear(command.SchemeYearId, nextSchemeYear.Id, cancellationToken, identityToken);
    }
}