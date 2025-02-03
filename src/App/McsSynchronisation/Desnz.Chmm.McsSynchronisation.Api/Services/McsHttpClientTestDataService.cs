using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Newtonsoft.Json;
using System.Net;

namespace Desnz.Chmm.McsSynchronisation.Api.Services
{
    public class McsHttpClientTestDataService : IMcsMidService
    {
        private readonly Random _random;
        private readonly IMcsTestDataCreateRepository _mcsDataRepository;
        private MscReferenceData? _referenceData;
        private int _systemId;

        private int GetNewSystemId() { return ++_systemId; }
        public int CurrentSystemId { get => _systemId; }

        public McsHttpClientTestDataService(IMcsTestDataCreateRepository mcsDataRepository)
        {
            _random = new Random();
            _referenceData = GetSampleReferenceData();
            _mcsDataRepository = mcsDataRepository;
            _systemId = _mcsDataRepository.GetMaxMidId().Result;
        }

        public async Task<HttpObjectResponse<McsInstallationsDto>> GetHeatPumpInstallations(GetMcsInstallationsDto content)
        {
            return await Task.Run(() =>
            {
                var installations = new List<McsInstallationDto>();

                for (int i = 0; i < 20000; i++)
                {
                    installations.Add(GetSingleInstallation(GetCommissioningDate(DateTime.Parse(content.StartDate), DateTime.Parse(content.EndDate)), GetRandomListItem(_referenceData.Manufacturers)));
                }
                return new HttpObjectResponse<McsInstallationsDto>(new HttpResponseMessage(HttpStatusCode.OK), new McsInstallationsDto { Installations = installations });
            });
        }

        public async Task<HttpObjectResponse<List<AirTypeTechnologyDto>>> GetAirTypeTechnologies()
        {
            return await Task.Run(() =>
            {
                return new HttpObjectResponse<List<AirTypeTechnologyDto>>(new HttpResponseMessage(HttpStatusCode.OK), _referenceData.AirTypeTechnologies);
            });
        }

        public async Task<HttpObjectResponse<List<AlternativeSystemFuelTypeDto>>> GetAlternativeHeatingSystemFuels()
        {
            return await Task.Run(() =>
            {
                return new HttpObjectResponse<List<AlternativeSystemFuelTypeDto>>(new HttpResponseMessage(HttpStatusCode.OK), _referenceData.AlternativeSystemFuelTypes);
            });
        }

        public async Task<HttpObjectResponse<List<AlternativeSystemTypeDto>>> GetAlternativeHeatingSystems()
        {
            return await Task.Run(() =>
            {
                return new HttpObjectResponse<List<AlternativeSystemTypeDto>>(new HttpResponseMessage(HttpStatusCode.OK), _referenceData.AlternativeSystemTypes);
            });
        }

        public async Task<HttpObjectResponse<List<InstallationAgeDto>>> GetInstallationAges()
        {
            return await Task.Run(() =>
            {
                return new HttpObjectResponse<List<InstallationAgeDto>>(new HttpResponseMessage(HttpStatusCode.OK), _referenceData.InstallationAges);
            });
        }

        public async Task<HttpObjectResponse<List<ManufacturerDto>>> GetManufacturers()
        {
            return await Task.Run(() =>
            {
                return new HttpObjectResponse<List<ManufacturerDto>>(new HttpResponseMessage(HttpStatusCode.OK), _referenceData.Manufacturers);
            });
        }

        public async Task<HttpObjectResponse<List<NewBuildOptionDto>>> GetNewBuildOptions()
        {
            return await Task.Run(() =>
            {
                return new HttpObjectResponse<List<NewBuildOptionDto>>(new HttpResponseMessage(HttpStatusCode.OK), _referenceData.NewBuildOptions);
            });

        }

        public async Task<HttpObjectResponse<List<RenewableSystemDesignDto>>> GetRenewableSystemDesigns()
        {
            return await Task.Run(() =>
            {
                return new HttpObjectResponse<List<RenewableSystemDesignDto>>(new HttpResponseMessage(HttpStatusCode.OK), _referenceData.RenewableSystemDesigns);
            });
        }
        public async Task<HttpObjectResponse<List<TechnologyTypeDto>>> GetTechnologyTypes()
        {
            return await Task.Run(() =>
            {
                return new HttpObjectResponse<List<TechnologyTypeDto>>(new HttpResponseMessage(HttpStatusCode.OK), _referenceData.TechnologyTypes);
            });
        }

        public async Task<HttpObjectResponse<List<McsProductDto>>> GetHeatPumpProducts()
        {
            return await Task.Run(() =>
            {
                return new HttpObjectResponse<List<McsProductDto>>(new HttpResponseMessage(HttpStatusCode.OK), GetProducts(_referenceData));
            });
        }

        private MscReferenceData? GetSampleReferenceData()
        {
            var data = JsonConvert.DeserializeObject<MscReferenceData>(File.ReadAllText(@"./SampleReferenceData/SampleReferenceData.json"));
            data.HeatPumpProducts = GetProducts(data);
            return data;
        }

        private List<McsProductDto> GetProducts(MscReferenceData data)
        {
            return data.Manufacturers.Select(GetSingleProduct).ToList();
        }

        private McsInstallationDto GetSingleInstallation(DateTime commissioningDate, ManufacturerDto manufacturer)
        {
            var singleInstallation = new McsInstallationDto
            {
                CommissioningDate = commissioningDate,
                HeatPumpProducts = new List<McsProductDto> { _referenceData.HeatPumpProducts.Single(x => x.ManufacturerId == manufacturer.Id) },
                IsAlternativeHeatingSystemPresent = GetIsAlternativeHeatingSystemPresent(),
                IsHybrid = GetRandomBoolean(),
                TotalCapacity = GetTotalCapacity(),

                AirTypeTechnologyId = GetRandomListItem(_referenceData.AirTypeTechnologies).Id,

                AlternativeHeatingAgeId = GetRandomListItem(_referenceData.InstallationAges).Id,
                AlternativeHeatingFuelId = GetRandomListItem(_referenceData.AlternativeSystemFuelTypes).Id,
                
                IsSystemSelectedAsMCSTechnology = GetRandomBoolean(),

                CertificatesCount = GetCertificatesCount(),

                MidId = GetNewSystemId(),
                IsNewBuildId = GetRandomListItem(_referenceData.NewBuildOptions).Id,
                RenewableSystemDesignId = GetRandomListItem(_referenceData.RenewableSystemDesigns).Id,
                TechnologyTypeId = GetRandomListItem(_referenceData.TechnologyTypes).Id,
                Mpan = GetMpan()
            };

            singleInstallation.AlternativeHeatingSystemId = singleInstallation.IsSystemSelectedAsMCSTechnology.Value ?
                GetRandomListItem(_referenceData.TechnologyTypes).Id :
                GetRandomListItem(_referenceData.AlternativeSystemTypes).Id;

            return singleInstallation;
        }

        private string GetMpan()
        {
            return "S 01 801 101 22 6130 5588 165";
        }

        private int GetCertificatesCount()
        {
            return _random.Next(1, 10);
        }

        private decimal GetTotalCapacity()
        {
            return (decimal)(_random.NextDouble() * (20 - 5) + 5);
        }

        private bool GetRandomBoolean()
        {
            return _random.NextDouble() > 0.5;
        }

        private bool GetIsAlternativeHeatingSystemPresent()
        {
            return _random.NextDouble() > 0.5;
        }

        private McsProductDto GetSingleProduct(ManufacturerDto manufacturer)
        {
            var currentId = GetNewSystemId();

            var product = new McsProductDto
            {
                Id = currentId,
                Code = $"{manufacturer.Description.Substring(0, 5).Replace(' ', '-').ToUpper()}-{currentId}-CODE",
                Name = $"{manufacturer.Description.Substring(0, 5).Replace(' ', '-').ToUpper()}-{currentId}-NAME",
                ManufacturerId = manufacturer.Id,
                ManufacturerName = manufacturer.Description
            };
            return product;
        }

        private DateTime GetCommissioningDate(DateTime startDate, DateTime endDate)
        {
            int range = (endDate - startDate).Days;
            return startDate.AddDays(_random.Next(range));
        }

        private T GetRandomListItem<T>(IEnumerable<T> list)
        {
            var values = list.ToArray();
            return values[_random.Next(values.Length)];
        }

        public void ResetIdSeed()
        {
            _systemId = 0;
        }


    }
}
