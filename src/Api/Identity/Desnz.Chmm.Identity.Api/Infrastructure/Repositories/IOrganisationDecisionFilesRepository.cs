using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories
{
    public interface IOrganisationDecisionFilesRepository : IRepository
    {
        Task<Guid> Create(OrganisationDecisionFile file, bool saveChanges = true);
        Task<List<OrganisationDecisionFile>> GetByOrganisationId(Guid organisationId, bool withTracking = false);
        Task<List<OrganisationDecisionFile>> GetByOrganisationIdAndDecision(Guid organisationId, string decision, bool withTracking = false);
    }
}
