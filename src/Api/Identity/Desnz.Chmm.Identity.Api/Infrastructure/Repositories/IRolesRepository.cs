using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;
using System.Linq.Expressions;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories;

public interface IRolesRepository : IRepository
{
    Task<List<ChmmRole>> GetAdminRoles(bool withTracking = false);
    Task<ChmmRole?> Get(Expression<Func<ChmmRole, bool>>? condition = null, bool withTracking = false);
    Task<ChmmRole?> GetByName(string roleName, bool withTracking = false);
    Task<List<ChmmRole>> GetAll(Expression<Func<ChmmRole, bool>>? condition = null, bool withTracking = false);
}
