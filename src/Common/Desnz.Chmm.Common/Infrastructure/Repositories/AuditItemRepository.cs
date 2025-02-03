using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Common.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for audit items
    /// </summary>
    public class AuditItemRepository : IAuditItemRepository
    {
        private readonly AuditingContext _context;

        /// <summary>
        /// Default constructor for the audit item repository
        /// </summary>
        /// <param name="context">System audit context</param>
        public AuditItemRepository(AuditingContext context)
        {
            _context = context;
        }

        public async Task<List<AuditItem>> GetAuditItemsForOrganisation(Guid organisationId)
        {
            return await _context.AuditItems
                .Where(a => a.OrganisationId == organisationId && a.AuditType == AuditTypeConstants.Command && a.WasSuccessful)
                .OrderByDescending(a => a.CreationDate)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Create a single audit event
        /// </summary>
        /// <param name="auditItem">The item to add</param>
        public async Task Create(AuditItem auditItem)
        {
            await _context.AuditItems.AddAsync(auditItem);
            await _context.SaveChangesAsync();
        }
    }

}
