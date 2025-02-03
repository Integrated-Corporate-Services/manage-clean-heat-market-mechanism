using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories;

public class RolesRepository : IRolesRepository
{
    private readonly IdentityContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public RolesRepository(IdentityContext context)
    {
        _context = context;
    }

    public async Task<List<ChmmRole>> GetAdminRoles(bool withTracking = false)
        => await Query(role =>
            role.Name == IdentityConstants.Roles.RegulatoryOfficer
            || role.Name == IdentityConstants.Roles.SeniorTechnicalOfficer
            || role.Name == IdentityConstants.Roles.PrincipalTechnicalOfficer,
            withTracking).OrderBy(r => r.Name).ToListAsync();

    public async Task<ChmmRole?> Get(Expression<Func<ChmmRole, bool>>? condition = null, bool withTracking = false)
        => await Query(condition, withTracking).SingleOrDefaultAsync();

    public async Task<ChmmRole?> GetByName(string roleName, bool withTracking = false)
        => await Query(r => r.Name == roleName, withTracking).SingleOrDefaultAsync();

    public async Task<List<ChmmRole>> GetAll(Expression<Func<ChmmRole, bool>>? condition = null, bool withTracking = false)
        => await Query(condition, withTracking).ToListAsync();

    private IQueryable<ChmmRole> Query(Expression<Func<ChmmRole, bool>>? condition = null, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.ChmmRoles :
            _context.ChmmRoles.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
}
