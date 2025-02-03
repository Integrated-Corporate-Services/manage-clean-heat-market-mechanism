using Desnz.Chmm.Common.ValueObjects;

namespace Desnz.Chmm.Common.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for audit items
    /// </summary>
    public interface IAuditItemRepository
    {
        /// <summary>
        /// Create a single audit event
        /// </summary>
        /// <param name="auditItem">The item to add</param>
        Task Create(AuditItem auditItem);
        Task<List<AuditItem>> GetAuditItemsForOrganisation(Guid organisationId);
    }

}
