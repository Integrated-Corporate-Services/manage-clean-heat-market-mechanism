using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Common.Commands;
using MediatR;
using Desnz.Chmm.Identity.Common.Queries;

namespace Desnz.Chmm.Identity.Api.Controllers;

/// <summary>
/// Identity API
/// </summary>
[ApiController]
[Route("api/identity")]
public class DateTimeController : ControllerBase
{
    private readonly IMediator _mediator;

    public DateTimeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns the current system date-time
    /// </summary>
    /// <param name="query">A query</param>
    /// <response code="200">The date tiem</response>
    [HttpGet("get-datetime")]
    public async Task<ActionResult<string>> GetDateTime(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetDateTimeQuery(), cancellationToken);
    }
}
