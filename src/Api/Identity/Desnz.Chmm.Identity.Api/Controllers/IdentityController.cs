using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Common.Commands;
using MediatR;

namespace Desnz.Chmm.Identity.Api.Controllers;

/// <summary>
/// Identity API
/// </summary>
[ApiController]
[Route("api/identity")]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Authenticate user and get JWT token
    /// </summary>
    /// <param name="command">Command with GOV.UK One Login information</param>
    /// <response code="200">JWT token</response>
    /// <response code="400">Invalid information in command</response>
    [HttpPost("token")]
    public async Task<ActionResult<string>> GetJwtToken([FromBody] GetJwtTokenCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}
