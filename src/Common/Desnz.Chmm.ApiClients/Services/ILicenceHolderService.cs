using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;

namespace Desnz.Chmm.ApiClients.Services
{
    /// <summary>
    /// Licence holder service allowing access to the Licence Holder services to other APIs
    /// </summary>
    public interface ILicenceHolderService
    {
        /// <summary>
        /// Get all licence holdesr
        /// </summary>
        /// <returns>A list of all licence holders</returns>
        Task<HttpObjectResponse<List<LicenceHolderDto>>> GetAll(string? token = null);

        /// <summary>
        /// Get all licence holder links
        /// </summary>
        /// <returns>A list of all licence holder links</returns>
        Task<HttpObjectResponse<List<LicenceHolderLinkDto>>> GetAllLinks(string? token = null);

        /// <summary>
        /// Get active licence holders for a given organisation
        /// </summary>
        /// <param name="organisationId">Organisation Id</param>
        /// <returns>A list of all licence holders for the given organisation</returns>
        Task<HttpObjectResponse<List<LicenceHolderLinkDto>>> GetLinkedTo(Guid organisationId);


        /// <summary>
        /// Get all licence holders for a given organisation
        /// </summary>
        /// <param name="organisationId">Organisation Id</param>
        /// <returns>A list of all licence holders for the given organisation</returns>
        Task<HttpObjectResponse<List<LicenceHolderLinkDto>>> GetLinkedToHistory(Guid organisationId);

        /// <summary>
        /// Create a licence holder
        /// </summary>
        /// <param name="command">Details of the licence holder to create</param>
        /// <returns>The Id of the created entity</returns>
        Task<HttpObjectResponse<CreatedResponse>> Create(CreateLicenceHolderCommand command, string? token = null);

        /// <summary>
        /// Create multiple licence holders
        /// </summary>
        /// <param name="command">Details of the licence holders to create</param>
        /// <returns>The Ids of the created entities</returns>
        Task<HttpObjectResponse<List<CreatedResponse>>> Create(CreateLicenceHoldersCommand command, string? token = null);
        Task<HttpObjectResponse<LicenceHolderExistsDto>> Exists(Guid licenceHolderId, string? token = null);
    }
}