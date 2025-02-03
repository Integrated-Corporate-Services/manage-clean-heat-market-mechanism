using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;

namespace Desnz.Chmm.ApiClients.Services
{
    public interface IOrganisationService
    {
        Task<HttpObjectResponse<EditOrganisationDto>> Get(Guid organisationId);
        Task<HttpObjectResponse<OrganisationStatusDto>> GetStatus(Guid organisationId);
        Task<HttpObjectResponse<List<ViewOrganisationDto>>> GetManufacturers(string? token = null);
        Task<HttpObjectResponse<List<OrganisationNameDto>>> GetManufacturerNames(OrganisationNameLookupQuery query, string? token = null);
        Task<HttpObjectResponse<List<ViewOrganisationDto>>> GetActiveManufacturers(string? token = null);
        Task<HttpObjectResponse<List<ViewOrganisationDto>>> GetOrganisationsAvailableForTransfer(Guid organisationId, string? token = null);
    }
}
