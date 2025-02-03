using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories
{
    public interface IOrganisationStructureFilesRepository : IRepository
    {
        Task<Guid> Create(OrganisationStructureFile file, bool saveChanges = true);
        IList<OrganisationStructureFile> GetForOrganisation(Guid organisationId);
        void Delete(OrganisationStructureFile organisationStructureFile);
    }
}
