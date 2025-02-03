using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Common.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Repository of licence holders
    /// </summary>
    public class LicenceHolderRepository : ILicenceHolderRepository
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
        public LicenceHolderRepository(IdentityContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return all licence holders matching the condition
        /// </summary>
        /// <param name="condition">Filter licence holders</param>
        /// <param name="withTracking">Should track?</param>
        /// <returns>A collection of licence holders</returns>
        public async Task<List<LicenceHolder>> GetAll(Expression<Func<LicenceHolder, bool>>? condition = null, bool includeLicenceHolderLinks = false, bool withTracking = false)
            => await Query(condition, includeLicenceHolderLinks, withTracking).OrderBy(u => u.Name).ToListAsync();

        /// <summary>
        /// Get a single licence holder
        /// </summary>
        /// <param name="condition">Licence holder filter</param>
        /// <param name="withTracking">Should track?</param>
        /// <returns>A single licence holder</returns>
        public async Task<LicenceHolder?> Get(Expression<Func<LicenceHolder, bool>>? condition = null, bool includeLicenceHolderLinks = false, bool withTracking = false)
            => await Query(condition, includeLicenceHolderLinks, withTracking).SingleOrDefaultAsync();

        /// <summary>
        /// Create a single licence holder
        /// </summary>
        /// <param name="licenceHolder">The licence holder to create</param>
        /// <param name="saveChanges">Should save changes?</param>
        /// <returns>The Id of the created entity</returns>
        public async Task<Guid> Create(LicenceHolder licenceHolder, bool saveChanges = true)
        {
            await _context.LicenceHolders.AddAsync(licenceHolder);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            return licenceHolder.Id;
        }

        private IQueryable<LicenceHolder> Query(Expression<Func<LicenceHolder, bool>>? condition = null, bool includeLicenceHolderLinks = false, bool withTracking = false)
        {
            var query = (condition == null) ?
                _context.LicenceHolders :
                _context.LicenceHolders.Where(condition);

            if (includeLicenceHolderLinks)
            {
                query = query.Include(u => u.LicenceHolderLinks);
            }

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        /// <summary>
        /// Add a range of Licesnse Holders
        /// This will ignore duplicates
        /// </summary>
        /// <param name="licenceHolders">The licence holders to add</param>
        /// <returns>Ok</returns>
        public async Task Append(IEnumerable<LicenceHolder> licenceHolders)
        {
            var existingLicenceHolders = _context.LicenceHolders.AsNoTracking().AsQueryable();
            var licenceHoldersToAdd = licenceHolders.Except(existingLicenceHolders);
            await _context.LicenceHolders.AddRangeAsync(licenceHoldersToAdd);
            await _context.SaveChangesAsync();
        }
    }
}
