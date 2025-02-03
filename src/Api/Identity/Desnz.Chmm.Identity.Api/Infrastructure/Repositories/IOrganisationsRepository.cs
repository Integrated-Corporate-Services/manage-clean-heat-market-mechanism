using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;
using System.Linq.Expressions;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories;

public interface IOrganisationsRepository : IRepository
{
    Task<IList<Organisation>> GetAll(Expression<Func<Organisation, bool>>? condition = null, bool includeLicenceHolderLinks = false, bool withTracking = false);
    Task<IList<Organisation>> GetAllActive(bool withTracking = false);
    Task<int> Count(Expression<Func<Organisation, bool>>? condition = null);
    Task<Organisation?> Get(Expression<Func<Organisation, bool>>? condition = null, List<Guid>? userIds = null, bool withTracking = false);
    Task<Organisation?> GetById(Guid Id, bool includeAddresses = false, bool includeUsers = false, bool includeLicenceHolderLinks = false, bool withTracking = false);
    Task<IOrganisation?> GetByIdForUpdate(Guid Id, bool includeAddresses = false, bool includeUsers = false, bool withTracking = false);
    Task<Guid> Create(Organisation organisation, bool saveChanges = true);
    Task<Guid> CreateAddress(Guid organisationId, OrganisationAddress organisationAddress, bool saveChanges = true);
}
