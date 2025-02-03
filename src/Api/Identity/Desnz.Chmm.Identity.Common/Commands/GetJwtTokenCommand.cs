using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands;

/// <summary>
/// Command to authenticate user and get JWT token
/// </summary>
[ExcludeFromAudit]
public class GetJwtTokenCommand : IRequest<ActionResult<string>>
{
    /// <summary>
    /// GOV.UK One Login ID token
    /// </summary>
    public string? IdToken { get; private set; }

    /// <summary>
    /// GOV.UK One Login access token
    /// </summary>
    public string? AccessToken { get; private set; }

    /// <summary>
    /// GOV.UK One Login email address
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// CHMM API Key
    /// </summary>
    public string? ApiKey { get; private set; }

    public GetJwtTokenCommand(string? idToken, string? accessToken, string? email = null, string? apiKey = null)
    {
        IdToken = idToken;
        AccessToken = accessToken;
        Email = email;
        ApiKey = apiKey;
    }
}
