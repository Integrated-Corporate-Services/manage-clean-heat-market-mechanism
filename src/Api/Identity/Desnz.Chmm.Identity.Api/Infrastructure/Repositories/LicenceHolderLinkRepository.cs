using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Repository of licence holders
    /// </summary>
    public class LicenceHolderLinkRepository : ILicenceHolderLinkRepository
    {
        private readonly IdentityContext _context;

        /// <summary>
        /// Unit of work
        /// </summary>
        public IUnitOfWork UnitOfWork => _context;

        /// <summary>
        /// DI constructor
        /// </summary>
        /// <param name="context">Db context</param>
        public LicenceHolderLinkRepository(IdentityContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return all licence holders matching the condition
        /// </summary>
        /// <param name="condition">Filter licence holders</param>
        /// <param name="withTracking">Should track?</param>
        /// <returns>A collection of licence holders</returns>
        public async Task<List<LicenceHolderLink>> GetAll(Expression<Func<LicenceHolderLink, bool>>? condition = null, bool includeLicenceHolder = false, bool includeOrganisation = false, bool withTracking = false)
            => await Query(condition, includeLicenceHolder, includeOrganisation, withTracking).OrderBy(u => u.LicenceHolder.Name).ToListAsync();

        /// <summary>
        /// Get a single licence holder
        /// </summary>
        /// <param name="condition">Licence holder filter</param>
        /// <param name="withTracking">Should track?</param>
        /// <returns>A single licence holder</returns>
        //public async Task<LicenceHolder?> Get(Expression<Func<LicenceHolder, bool>>? condition = null, bool withTracking = false)
        //    => await Query(condition, withTracking).SingleOrDefaultAsync();

        /// <summary>
        /// Create a single licence holder link
        /// </summary>
        /// <param name="licenceHolder">The licence holder to create</param>
        /// <param name="saveChanges">Should save changes?</param>
        /// <returns>The Id of the created entity</returns>
        public async Task<Guid> Create(LicenceHolderLink licenceHolderLink, bool saveChanges = true)
        {
            await _context.LicenceHolderLinks.AddAsync(licenceHolderLink);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            return licenceHolderLink.Id;
        }

        private IQueryable<LicenceHolderLink> Query(Expression<Func<LicenceHolderLink, bool>>? condition = null, bool includeLicenceHolder = false, bool includeOrganisation = false, bool withTracking = false)
        {
            var query = (condition == null) ?
                _context.LicenceHolderLinks :
                _context.LicenceHolderLinks.Where(condition);

            if (includeLicenceHolder)
            {
                query = query.Include(u => u.LicenceHolder);
            }

            if (includeOrganisation)
            {
                query = query.Include(u => u.Organisation);
            }

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }
    }
}
