using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories
{
    public class OrganisationDecisionFilesRepository : IOrganisationDecisionFilesRepository
    {
        private readonly IdentityContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public OrganisationDecisionFilesRepository(IdentityContext context)
        {
            _context = context;
        }

        public async Task<Guid> Create(OrganisationDecisionFile file, bool saveChanges = true)
        {
            await _context.OrganisationDecisionFiles.AddAsync(file);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            return file.Id;
        }

        public async Task<List<OrganisationDecisionFile>> GetByOrganisationId(Guid organisationId, bool withTracking = false)
            => await Query(i => i.OrganisationId == organisationId, withTracking).ToListAsync();


        public async Task<List<OrganisationDecisionFile>> GetByOrganisationIdAndDecision(Guid organisationId, string decision, bool withTracking = false)
            => await Query(i => i.OrganisationId == organisationId && i.Decision == decision, withTracking).ToListAsync();

        private IQueryable<Entities.OrganisationDecisionFile> Query(Expression<Func<Entities.OrganisationDecisionFile, bool>>? condition = null, bool withTracking = false)
        {
            var query = (condition == null) ?
                _context.OrganisationDecisionFiles :
                _context.OrganisationDecisionFiles.Where(condition);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }
    }
}
