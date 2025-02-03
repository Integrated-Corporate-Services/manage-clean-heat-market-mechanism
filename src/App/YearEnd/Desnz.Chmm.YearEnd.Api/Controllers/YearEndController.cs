using Desnz.Chmm.Common.Authorization.Constants;
using Desnz.Chmm.Common.Swagger;
using Desnz.Chmm.YearEnd.Common.Commands;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.YearEnd.Api.Controllers;

/// <summary>
/// Provides the MCS data manipulation endpoints
/// </summary>
[ApiController]
[Route("api/yearend")]
public class YearEndController : ControllerBase
{
    private readonly ILogger<YearEndController> _logger;
    private readonly IMediator _mediator;

    public YearEndController(
        ILogger<YearEndController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [SwaggerOperationFilter(typeof(ApiKeyHeaderFilter))]
    [HttpPost("process-end-of-year")]
    [Authorize(Policy = AuthorizationConstants.RequireApiKeyPolicy)]
    public async Task<ActionResult> ProcessEndOfYear(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new ProcessEndOfYearCommand(), cancellationToken);
    }


    [SwaggerOperationFilter(typeof(ApiKeyHeaderFilter))]
    [HttpPost("process-end-of-year/rollback")]
    [Authorize(Policy = AuthorizationConstants.RequireApiKeyPolicy)]
    public async Task<ActionResult> RollbackEndOfYear([FromBody] RollbackEndOfYearCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [SwaggerOperationFilter(typeof(ApiKeyHeaderFilter))]
    [HttpPost("redemption")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.ApiRole)]
    public async Task<ActionResult> ProcessRedemption([FromBody] ProcessRedemptionCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [SwaggerOperationFilter(typeof(ApiKeyHeaderFilter))]
    [HttpPost("redemption/rollback")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.ApiRole)]
    public async Task<ActionResult> RollbackProcessRedemption([FromBody] RollbackRedemptionCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}