using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Authorization.Constants;
using Desnz.Chmm.McsSynchronisation.Common.Commands;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using Desnz.Chmm.Common.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Desnz.Chmm.Common.Authorization.Controllers;

/// <summary>
/// Provides the MCS data manipulation endpoints
/// </summary>
[ApiController]
[Route("api/mcssynchronisation")]
public class McsSynchronisationController : ControllerBase
{
    private readonly ILogger<McsSynchronisationController> _logger;
    private readonly IMediator _mediator;

    public McsSynchronisationController(
        ILogger<McsSynchronisationController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Gets the MCS installation data
    /// </summary>
    /// <param name="manufacturerId"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("organisation/{manufacturerId}/{startDate}/{endDate}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.ApiRole)]
    public async Task<ActionResult<List<CreditCalculationDto>>> GetManufacturerInstallations(int manufacturerId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var query = new GetManufacturerInstallationsQuery(manufacturerId, startDate, endDate);
        return await _mediator.Send(query, cancellationToken);
    }

    [HttpGet("installation-request/{installationRequestId}/{pageNumber:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.AdminsAndApi)]
    public async Task<ActionResult<DataPage<CreditCalculationDto>>> GetInstallationsPerRequest(Guid installationRequestId, int pageNumber, CancellationToken cancellationToken)
    {
        var query = new GetInstallationRequestQuery(installationRequestId, pageNumber);
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Trggers the MCS data fetching and persisting 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [SwaggerOperationFilter(typeof(ApiKeyHeaderFilter))]
    [HttpPost("manual-mcsdata-import")]
    [Authorize(Policy = AuthorizationConstants.RequireApiKeyPolicy)]
    public async Task<ActionResult> ManualMcsDataImport([FromBody] InstallationRequestCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }


    /// <summary>
    /// Trggers the MCS data fetching and persisting 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [SwaggerOperationFilter(typeof(ApiKeyHeaderFilter))]
    [HttpPost("trigger-mcsdata-import")]
    [Authorize(Policy = AuthorizationConstants.RequireApiKeyPolicy)]
    public async Task<ActionResult> TriggerMcsDataImport([FromBody] SynchroniseInstallationsCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}