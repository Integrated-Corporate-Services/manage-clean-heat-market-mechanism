using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;
using System.Linq.Expressions;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Repository of licence holders
    /// </summary>
    public interface ILicenceHolderRepository : IRepository
    {
        /// <summary>
        /// Return all licence holders matching the condition
        /// </summary>
        /// <param name="condition">Filter licence holders</param>
        /// <param name="withTracking">Should track?</param>
        /// <returns>A collection of licence holders</returns>
        Task<List<LicenceHolder>> GetAll(Expression<Func<LicenceHolder, bool>>? condition = null, bool includeLicenceHolderLinks = false, bool withTracking = false);

        /// <summary>
        /// Get a single licence holder
        /// </summary>
        /// <param name="condition">Licence holder filter</param>
        /// <param name="withTracking">Should track?</param>
        /// <returns>A single licence holder</returns>
        Task<LicenceHolder?> Get(Expression<Func<LicenceHolder, bool>>? condition = null, bool includeLicenceHolderLinks = false, bool withTracking = false);

        /// <summary>
        /// Create a single licence holder
        /// </summary>
        /// <param name="licenceHolder">The licence holder to create</param>
        /// <param name="saveChanges">Should save changes?</param>
        /// <returns>The Id of the created entity</returns>
        Task<Guid> Create(LicenceHolder licenceHolder, bool saveChanges = true);

        /// <summary>
        /// Create a collection of licence holders
        /// </summary>
        /// <param name="licenceHolders">The creation information for licence holders</param>
        /// <returns>Ok</returns>
        Task Append(IEnumerable<LicenceHolder> licenceHolders);
    }
}
