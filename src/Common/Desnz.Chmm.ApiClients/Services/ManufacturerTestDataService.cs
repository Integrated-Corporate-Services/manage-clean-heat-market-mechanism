using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Desnz.Chmm.ApiClients.Services
{
    public class ManufacturerTestDataService : HttpServiceClient, IManufacturerService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// </summary>
        public ManufacturerTestDataService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }

        public async Task<HttpObjectResponse<List<ProductLicenceHolderDto>>> GetProductLicenceHolders()
        {
            return new HttpObjectResponse<List<ProductLicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ProductLicenceHolderDto>
            {
                new(Guid.NewGuid(), 1),
                new(Guid.NewGuid(), 2),
                new(Guid.NewGuid(), 3)
            }, null);

            return await HttpGetAsync<List<ProductLicenceHolderDto>>("api/identity/manufacturers/productlicenceholders");
        }
    }
}