
using Microsoft.EntityFrameworkCore;
using Desnz.Chmm.Common.Constants;
using System.Linq.Expressions;
using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IdentityContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public UsersRepository(IdentityContext context)
        {
            _context = context;
        }

        public async Task<List<ChmmUser>> GetAdmins(bool withTracking = false)
            => await Query(x => x.ChmmRoles.Any(r =>
                r.Name == IdentityConstants.Roles.RegulatoryOfficer ||
                r.Name == IdentityConstants.Roles.SeniorTechnicalOfficer ||
                r.Name == IdentityConstants.Roles.PrincipalTechnicalOfficer
            ), true, withTracking).ToListAsync();

        public async Task<List<ChmmUser>> GetAll(Expression<Func<ChmmUser, bool>>? condition = null, bool withTracking = false)
            => await Query(condition, false, withTracking).OrderBy(u => u.Name).ToListAsync();

        public async Task<ChmmUser?> GetBySubject(string subject, bool includeRoles = false, bool withTracking = false)
            => await Query(u => u.Subject == subject, includeRoles, withTracking).SingleOrDefaultAsync();

        public async Task<ChmmUser?> GetByEmail(string email, bool includeRoles = false, bool withTracking = false)
            => await Query(u => u.Email == email, includeRoles, withTracking).SingleOrDefaultAsync();

        public async Task<ChmmUser?> GetById(Guid id, bool includeRoles = false, bool withTracking = false)
            => await Query(u => u.Id == id, includeRoles, withTracking).SingleOrDefaultAsync();

        public async Task<ChmmUser?> Get(Expression<Func<ChmmUser, bool>>? condition = null, bool includeRoles = false, bool withTracking = false)
            => await Query(condition, includeRoles, withTracking).SingleOrDefaultAsync();

        public async Task<Guid> Create(ChmmUser user, bool saveChanges = true)
        {
            await _context.ChmmUsers.AddAsync(user);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            return user.Id;
        }

        private IQueryable<ChmmUser> Query(Expression<Func<ChmmUser, bool>>? condition = null, bool includeRoles = false, bool withTracking = false)
        {
            var query = (condition == null) ?
                _context.ChmmUsers :
                _context.ChmmUsers.Where(condition);

            if (includeRoles)
            {
                query = query.Include(u => u.ChmmRoles);
            }

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }
    }
}