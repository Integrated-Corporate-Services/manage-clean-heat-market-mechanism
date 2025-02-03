using Desnz.Chmm.Common;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using System.Linq.Expressions;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;

/// <summary>
/// A repository for dealing with Installation Requests
/// </summary>
public interface IInstallationRequestRepository : IRepository
{
    /// <summary>
    /// Query the list of installation requests
    /// </summary>
    /// <param name="condition">The query parameters</param>
    /// <param name="withTracking">Should track?</param>
    /// <returns>All installation requests matching the query</returns>
    Task<IList<InstallationRequest>> GetAll(Expression<Func<InstallationRequest, bool>>? condition = null, bool withTracking = false);

    /// <summary>
    /// Get a single Installation request by Id
    /// </summary>
    /// <param name="id">The installation request id</param>
    /// <param name="withTracking">Shoudl track?</param>
    /// <returns>The discovered item</returns>
    Task<InstallationRequest?> Get(Guid? id, bool withTracking = false);
}
