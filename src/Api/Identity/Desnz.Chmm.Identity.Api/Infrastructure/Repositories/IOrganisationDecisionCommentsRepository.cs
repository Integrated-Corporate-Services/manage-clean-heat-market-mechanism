using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories
{
    public interface IOrganisationDecisionCommentsRepository : IRepository
    {
        Task<Guid> Create(OrganisationDecisionComment comment, bool saveChanges = true);
        Task<OrganisationDecisionComment> GetByOrganisationId(Guid organisationId, bool withTracking = false);
        Task<OrganisationDecisionComment> GetByOrganisationIdAndDecision(Guid organisationId, string decision, bool withTracking = false);
    }
}
