using Desnz.Chmm.Common;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;

/// <summary>
/// A repository for dealing with Installation Requests
/// </summary>
public class InstallationRequestRepository : IInstallationRequestRepository
{
    /// <summary>
    /// Database context
    /// </summary>
    protected readonly McsSynchronisationContext _context;
    
    /// <summary>
    /// Unit of work
    /// </summary>
    public IUnitOfWork UnitOfWork => _context;

    /// <summary>
    /// DI constructor
    /// </summary>
    /// <param name="context">Database context</param>
    public InstallationRequestRepository(McsSynchronisationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Query the list of installation requests
    /// </summary>
    /// <param name="condition">The query parameters</param>
    /// <param name="withTracking">Should track?</param>
    /// <returns>All installation requests matching the query</returns>
    public async Task<IList<InstallationRequest>> GetAll(Expression<Func<InstallationRequest, bool>>? condition = null, bool withTracking = false)
        => await Query(condition, withTracking).OrderByDescending(o => o.RequestDate).ToListAsync();

    /// <summary>
    /// Get a single Installation request by Id
    /// </summary>
    /// <param name="id">The installation request id</param>
    /// <param name="withTracking">Shoudl track?</param>
    /// <returns>The discovered item</returns>
    public async Task<InstallationRequest?> Get(Guid? id, bool withTracking = false)
        => await Query(i => i.Id == id, withTracking).SingleOrDefaultAsync();

    private IQueryable<InstallationRequest> Query(Expression<Func<InstallationRequest, bool>>? condition = null, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.InstallationRequests :
            _context.InstallationRequests.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
}
