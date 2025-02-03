using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Configuration.Common.Commands;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.YearEnd.Common.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Desnz.Chmm.YearEnd.Api.Handlers.Commands;

public abstract class ProcessEndOfYearBase<TRequest, TResponse> : BaseRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected readonly ICreditLedgerService CreditLedgerService;
    protected readonly IObligationService ObligationService;
    protected readonly ISchemeYearService SchemeYearService;
    protected readonly IYearEndService EndOfYearService;

    private readonly IIdentityService _identityService;
    private readonly ApiKeyPolicyConfig _apiKeyPolicyConfig;
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected ProcessEndOfYearBase(ILogger<BaseRequestHandler<TRequest, TResponse>> logger,
    ICreditLedgerService creditLedgerService,
    IObligationService obligationService,
    ISchemeYearService schemeYearService,
    IIdentityService identityService,
    IYearEndService endOfYearService,
    IOptions<ApiKeyPolicyConfig> apiKeyPolicyConfig,
    IHttpContextAccessor httpContextAccessor) : base(logger)
    {
        CreditLedgerService = creditLedgerService;
        ObligationService = obligationService;
        SchemeYearService = schemeYearService;
        _identityService = identityService;
        EndOfYearService = endOfYearService;
        _apiKeyPolicyConfig = apiKeyPolicyConfig.Value
            ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.ApiKeyPolicy));
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Rolls back the End-of-Year process
    /// </summary>
    /// <param name="schemeYearId"></param>
    /// <param name="nextSchemeYearId"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="identityToken"></param>
    /// <returns></returns>
    protected async Task<ActionResult> RollbackEndOfYear(Guid schemeYearId, Guid nextSchemeYearId, CancellationToken cancellationToken, string identityToken)
    {
        var rollbackTasks = new List<Task<HttpObjectResponse<object>>>
            {
                SchemeYearService.RollbackGenerateNextYearsSchemea(new RollbackGenerateNextYearsSchemeCommand(nextSchemeYearId), cancellationToken, identityToken),
                CreditLedgerService.RollbackCarryOverCredit(new RollbackCarryOverCreditCommand(schemeYearId), cancellationToken, identityToken),
                ObligationService.RollbackCarryForwardObligation(new RollbackCarryForwardObligationCommand(schemeYearId), cancellationToken, identityToken),
                EndOfYearService.RollbackRedemption(new RollbackRedemptionCommand(schemeYearId), identityToken)
            };

        var result = await Task.WhenAll(rollbackTasks);
        if (result.Any(x => !x.IsSuccessStatusCode))
            return RollingBackEndOfYearProcessFailed(result.First(x => !x.IsSuccessStatusCode).Problem);
        return Responses.Ok();
    }

    /// <summary>
    /// Gets the identity token
    /// </summary>
    /// <returns></returns>
    protected async Task<ActionResult<string>> GetIdentityToken()
    {
        var apiKey = _httpContextAccessor.HttpContext?.Request.Headers[_apiKeyPolicyConfig.HeaderName].FirstOrDefault();
        var response = await _identityService.GetJwtToken(new GetJwtTokenCommand(null, null, null, apiKey));
        return !response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(response.Result) ?
            (ActionResult<string>)CannotLoadJwtToken(response.Problem) :
            (ActionResult<string>)response.Result;
    }
}
