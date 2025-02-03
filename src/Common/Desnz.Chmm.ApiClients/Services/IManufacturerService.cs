using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;

namespace Desnz.Chmm.ApiClients.Services
{
    public interface IManufacturerService
    {
        Task<HttpObjectResponse<List<ProductLicenceHolderDto>>> GetProductLicenceHolders();
    }
}
