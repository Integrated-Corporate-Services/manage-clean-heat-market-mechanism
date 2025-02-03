
using Desnz.Chmm.Identity.Api.Entities;

namespace Desnz.Chmm.Identity.Api.Services
{
    public interface ITokenService
    {
        Task ValidateTokenAsync(string idToken, CancellationToken cancellationToken);
        string GenerateJwtToken(string email, ChmmUser? user, string? apiKey = null);
    }
}
