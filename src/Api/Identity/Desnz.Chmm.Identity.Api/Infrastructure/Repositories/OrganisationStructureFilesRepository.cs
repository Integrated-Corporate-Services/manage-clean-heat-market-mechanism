using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories
{
    public class OrganisationStructureFilesRepository : IOrganisationStructureFilesRepository
    {
        private readonly IdentityContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public OrganisationStructureFilesRepository(IdentityContext context)
        {
            _context = context;
        }

        public async Task<Guid> Create(OrganisationStructureFile file, bool saveChanges = true)
        {
            await _context.OrganisationStructureFiles.AddAsync(file);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            return file.Id;
        }

        public void DeleteForOrganisation(Guid organisationId)
        {
            var files = _context.OrganisationStructureFiles.Where(f => f.OrganisationId == organisationId).ToList();

        }

        public IList<OrganisationStructureFile> GetForOrganisation(Guid organisationId)
        {
            return _context.OrganisationStructureFiles.Where(f => f.OrganisationId == organisationId).ToList();
        }

        public void Delete(OrganisationStructureFile organisationStructureFile)
        {
            _context.OrganisationStructureFiles.Remove(organisationStructureFile);
        }
    }
}
