using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;
using System.Linq.Expressions;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Repository of licence holders links
    /// </summary>
    public interface ILicenceHolderLinkRepository : IRepository
    {
        Task<Guid> Create(LicenceHolderLink licenceHolderLink, bool saveChanges = true);
    }
}
