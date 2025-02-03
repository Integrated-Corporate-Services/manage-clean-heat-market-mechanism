using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.ApiClients.Services
{
    /// <summary>
    /// Licence holder service allowing access to the Licence Holder services to other APIs
    /// </summary>
    public class LicenceHolderService : HttpServiceClient, ILicenceHolderService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// 
        /// DI Constructor
        /// </summary>
        /// <param name="client">Http Client</param>
        /// <param name="httpContextAccessor">Context Accessor</param>
        public LicenceHolderService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }

        /// <summary>
        /// Create a licence holder
        /// </summary>
        /// <param name="command">Details of the licence holder to create</param>
        /// <returns>The Id of the created entity</returns>
        public async Task<HttpObjectResponse<CreatedResponse>> Create(CreateLicenceHolderCommand command, string? token = null)
        {
            return await HttpPostAsync<CreatedResponse>("api/identity/licenceholders", command, token: token);
        }

        /// <summary>
        /// Create multiple licence holders
        /// </summary>
        /// <param name="command">Details of the licence holders to create</param>
        /// <returns>The Ids of the created entities</returns>
        public async Task<HttpObjectResponse<List<CreatedResponse>>> Create(CreateLicenceHoldersCommand command, string? token = null)
        {
            return await HttpPostAsync<List<CreatedResponse>>("api/identity/licenceholders/batch", command, token: token);
        }

        /// <summary>
        /// Get all licence holders
        /// </summary>
        /// <returns>A list of all licence holders</returns>
        public async Task<HttpObjectResponse<List<LicenceHolderDto>>> GetAll(string? token = null)
        {
            return await HttpGetAsync<List<LicenceHolderDto>>("/api/identity/licenceholders/all", token: token);
        }

        /// <summary>
        /// Get all licence holders
        /// </summary>
        /// <returns>A list of all licence holders</returns>
        public async Task<HttpObjectResponse<LicenceHolderExistsDto>> Exists(Guid licenceHolderId, string? token = null)
        {
            return await HttpGetAsync<LicenceHolderExistsDto>($"/api/identity/licenceholders/exists/{licenceHolderId}", token: token);
        }

        /// <summary>
        /// Get all licence holder links
        /// </summary>
        /// <returns>A list of all licence holder links</returns>
        public async Task<HttpObjectResponse<List<LicenceHolderLinkDto>>> GetAllLinks(string? token = null)
        {
            return await HttpGetAsync<List<LicenceHolderLinkDto>>("/api/identity/licenceholders/links/all", token: token);
        }

        /// <summary>
        /// Get all licence holders for an organisation
        /// </summary>
        /// <param name="organisationId">The organisation to query</param>
        /// <returns>A list of all licence holders for the organisation</returns>
        public async Task<HttpObjectResponse<List<LicenceHolderLinkDto>>> GetLinkedTo(Guid organisationId)
        {
            return await HttpGetAsync<List<LicenceHolderLinkDto>>($"api/identity/licenceholders/linked-to/{organisationId}");
        }

        /// <summary>
        /// Get all licence holders for an organisation
        /// </summary>
        /// <param name="organisationId">The organisation to query</param>
        /// <returns>A list of all licence holders for the organisation</returns>
        public async Task<HttpObjectResponse<List<LicenceHolderLinkDto>>> GetLinkedToHistory(Guid organisationId)
        {
            return await HttpGetAsync<List<LicenceHolderLinkDto>>($"api/identity/licenceholders/linked-to/{organisationId}/all");
        }
    }
}