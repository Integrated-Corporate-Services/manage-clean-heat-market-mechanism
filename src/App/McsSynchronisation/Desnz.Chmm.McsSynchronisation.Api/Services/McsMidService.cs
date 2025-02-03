
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.McsSynchronisation.Api.Configuration.Settings;
using Desnz.Chmm.McsSynchronisation.Common.Commands;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;


namespace Desnz.Chmm.McsSynchronisation.Api.Services
{
    public class McsMidService : IMcsMidService
    {
        private readonly McsApiConfig _mcsApiConfig;
        private readonly HttpClient _httpClient;

        public McsMidService(
            IOptions<McsApiConfig> mcsApiConfig, 
            HttpClient httpClient)
        {
            _mcsApiConfig = mcsApiConfig.Value
                    ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.McsApi));
            _httpClient = httpClient;
        }

        public async Task<HttpObjectResponse<McsInstallationsDto>> GetHeatPumpInstallations(GetMcsInstallationsDto content)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "chmm/heat-pump-installations");
            return await HttpSendAsync<McsInstallationsDto>(request, content);
        }

        public async Task<HttpObjectResponse<McsProductsDto>> GetProducts()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "chmm/products");
            return await HttpSendAsync<McsProductsDto>(request);
        }

        public async Task<HttpObjectResponse<MscReferenceDataDto>> GetTechnologyTypes()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "chmm/technology-types");
            return await HttpSendAsync<MscReferenceDataDto>(request);
        }

        public async Task<HttpObjectResponse<MscReferenceDataDto>> GetAirTypeTechnologies()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "chmm/air-type-technologies");
            return await HttpSendAsync<MscReferenceDataDto>(request);
        }

        public async Task<HttpObjectResponse<MscReferenceDataDto>> GetAlternativeHeatingSystems()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "chmm/alternative-heating-systems");
            return await HttpSendAsync<MscReferenceDataDto>(request);
        }

        public async Task<HttpObjectResponse<MscReferenceDataDto>> GetInstallationAges()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "chmm/installation-ages");
            return await HttpSendAsync<MscReferenceDataDto>(request);
        }

        public async Task<HttpObjectResponse<MscReferenceDataDto>> GetNewBuildOptions()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "chmm/new-build-options");
            return await HttpSendAsync<MscReferenceDataDto>(request);
        }

        public async Task<HttpObjectResponse<MscReferenceDataDto>> GetManufacturers()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "chmm/manufacturers");
            return await HttpSendAsync<MscReferenceDataDto>(request);
        }

        public async Task<HttpObjectResponse<MscReferenceDataDto>> GetRenewableSystemDesigns()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "chmm/renewable-system-design");
            return await HttpSendAsync<MscReferenceDataDto>(request);
        }

        public async Task<HttpObjectResponse<MscReferenceDataDto>> GetAlternativeHeatingSystemFuels()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "chmm/alternative-heating-system-fuels");
            return await HttpSendAsync<MscReferenceDataDto>(request);
        }

        private async Task<HttpObjectResponse<T>> HttpSendAsync<T>(HttpRequestMessage request, object? content = null)
            where T : class
        {
            request.Headers.Add("Authorization", _mcsApiConfig.ApiKey);
            request.Headers.Add("User", _mcsApiConfig.Email);

            if (content != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);

            var result = await HttpObjectResponseFactory.DetermineSuccess<T>(response);
            return result;
        }
    }
}
