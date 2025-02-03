using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories;

public interface IUsersRepository : IRepository
{
    Task<List<ChmmUser>> GetAdmins(bool withTracking = false);
    Task<List<ChmmUser>> GetAll(Expression<Func<ChmmUser, bool>>? condition = null, bool withTracking = false);

    Task<ChmmUser?> GetBySubject(string subject, bool includeRoles = false, bool withTracking = false);
    Task<ChmmUser?> GetByEmail(string email, bool includeRoles = false, bool withTracking = false);
    Task<ChmmUser?> GetById(Guid id, bool includeRoles = false, bool withTracking = false);
    Task<ChmmUser?> Get(Expression<Func<ChmmUser, bool>>? condition = null, bool includeRoles = false, bool withTracking = false);
    Task<Guid> Create(ChmmUser user, bool saveChanges = true);
}
