using Desnz.Chmm.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories
{
    public class OrganisationDecisionCommentsRepository : IOrganisationDecisionCommentsRepository
    {
        private readonly IdentityContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public OrganisationDecisionCommentsRepository(IdentityContext context)
        {
            _context = context;
        }

        public async Task<Guid> Create(Entities.OrganisationDecisionComment comment, bool saveChanges = true)
        {
            await _context.OrganisationDecisionComments.AddAsync(comment);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            return comment.Id;
        }

        public async Task<Entities.OrganisationDecisionComment> GetByOrganisationId(Guid organisationId, bool withTracking = false)
            => await Query(i => i.OrganisationId == organisationId, withTracking).FirstOrDefaultAsync();

        public async Task<Entities.OrganisationDecisionComment> GetByOrganisationIdAndDecision(Guid organisationId, string decision, bool withTracking = false)
            => await Query(i => i.OrganisationId == organisationId && i.Decision == decision, withTracking).Include(i => i.ChmmUser).FirstOrDefaultAsync();

        private IQueryable<Entities.OrganisationDecisionComment> Query(Expression<Func<Entities.OrganisationDecisionComment, bool>>? condition = null, bool withTracking = false)
        {
            var query = (condition == null) ?
                _context.OrganisationDecisionComments :
                _context.OrganisationDecisionComments.Where(condition);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }
    }
}
