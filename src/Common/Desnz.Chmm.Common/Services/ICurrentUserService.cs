using System.Security.Claims;

namespace Desnz.Chmm.Common.Services
{
    /// <summary>
    /// User service to get the current user
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// The current claims principal
        /// </summary>
        ClaimsPrincipal? CurrentUser { get; }
    }
}