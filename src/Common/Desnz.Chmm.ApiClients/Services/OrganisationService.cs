using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.ApiClients.Services
{
    public class OrganisationService : HttpServiceClient, IOrganisationService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// </summary>
        public OrganisationService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }

        public async Task<HttpObjectResponse<EditOrganisationDto>> Get(Guid organisationId)
        {
            return await HttpGetAsync<EditOrganisationDto>($"api/identity/organisations/{organisationId}");
        }

        public async Task<HttpObjectResponse<OrganisationStatusDto>> GetStatus(Guid organisationId)
        {
            return await HttpGetAsync<OrganisationStatusDto>($"api/identity/organisations/{organisationId}/status");
        }

        public async Task<HttpObjectResponse<List<ViewOrganisationDto>>> GetManufacturers(string? token = null)
        {
            return await HttpGetAsync<List<ViewOrganisationDto>>($"api/identity/organisations/", token: token);
        }

        public async Task<HttpObjectResponse<List<OrganisationNameDto>>> GetManufacturerNames(OrganisationNameLookupQuery query, string? token = null)
        {
            return await HttpGetAsync<List<OrganisationNameDto>>($"api/identity/organisations/name-lookup", query: query, token: token);
        }

        public async Task<HttpObjectResponse<List<ViewOrganisationDto>>> GetActiveManufacturers(string? token = null)
        {
            return await HttpGetAsync<List<ViewOrganisationDto>>($"api/identity/organisations/active", token: token);
        }

        public async Task<HttpObjectResponse<List<ViewOrganisationDto>>> GetOrganisationsAvailableForTransfer(Guid organisationId, string? token = null)
        {
            return await HttpGetAsync<List<ViewOrganisationDto>>($"api/identity/organisations/{organisationId}/available-for-transfer", token: token);
        }
    }
}
