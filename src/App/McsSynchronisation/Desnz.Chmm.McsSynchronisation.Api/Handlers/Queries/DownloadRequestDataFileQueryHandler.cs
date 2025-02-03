using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace Desnz.Chmm.McsSynchronisation.Api.Handlers.Queries
{
    /// <summary>
    /// Handle downloading all requested Mcs install installations
    /// </summary>
    public class DownloadRequestDataFileQueryHandler : BaseRequestHandler<DownloadRequestDataFileQuery, ActionResult<Stream>>
    {
        /// <summary>
        /// Mapping class for Csv Helper to successfully map a HeatPumpInstallation for download
        /// </summary>
        private sealed class HeatPumpInstallationMap : ClassMap<HeatPumpInstallation>
        {
            /// <summary>
            /// Mapping for MCS installations download
            /// </summary>
            private HeatPumpInstallationMap()
            {
                Map(m => m.MidId).Name("ID");
                Map(m => m.TechnologyTypeId).Name("TechnologyTypeID");
                Map(m => m.AirTypeTechnologyId).Name("AirTypeTechnologyID");
                Map(m => m.IsAlternativeHeatingSystemPresent).Name("IsAlternativeHeatingSystemPresent");
                Map(m => m.IsSystemSelectedAsMCSTechnology).Name("IsSystemSelectedAsMCSTechnology");
                Map(m => m.AlternativeHeatingSystemId).Name("AlternativeHeatingSystemID");
                Map(m => m.AlternativeHeatingFuelId).Name("AlternativeHeatingFuelID");
                Map(m => m.AlternativeHeatingAgeId).Name("AlternativeHeatingAgeID");
                Map(m => m.CommissioningDate).TypeConverter<DateConverter>().Name("CommissioningDate");
                Map(m => m.Mpan).Name("MPAN");
                Map(m => m.TotalCapacity).Name("TotalCapacity");
                Map(m => m.CertificatesCount).Name("HowManyCertificates");
                Map(m => m.IsHybrid).Name("IsHybrid");
                Map(m => m.IsNewBuildId).Name("IsNewBuildID");
                Map(m => m.RenewableSystemDesignId).Name("RenewableSystemDesignID");
                Map(m => m.HeatPumpProducts).TypeConverter<HeatPumpProductsConverter>().Name("HeatPumpProducts");
                Map(m => m.Credits).Name("Credits Generated");
            }
        }

        /// <summary>
        /// Converter to turn the list of heat pump products into a string of values
        /// </summary>
        private class DateConverter : DefaultTypeConverter
        {
            /// <summary>
            /// Convert to the defined format
            /// </summary>
            /// <param name="value">The List(HeatPumpProduct)</param>
            /// <param name="row">The IWriterRow</param>
            /// <param name="memberMapData">Mapping installations</param>
            /// <returns>A string of values</returns>
            public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
            {
                var data = value as DateTime?;
                if (data == null) return "";

                return data.Value.ToString("dd/MM/yyyy");
            }
        }

        /// <summary>
        /// Converter to turn the list of heat pump products into a string of values
        /// </summary>
        private class HeatPumpProductsConverter : DefaultTypeConverter
        {
            /// <summary>
            /// Convert to the defined format
            /// </summary>
            /// <param name="value">The List(HeatPumpProduct)</param>
            /// <param name="row">The IWriterRow</param>
            /// <param name="memberMapData">Mapping installations</param>
            /// <returns>A string of values</returns>
            public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
            {
                var data = value as List<HeatPumpProduct>;
                if (data == null) return "";

                var output = data.Select(p => $"ID: {p.Id} | MCS Product Number: {p.Code} | Product Name: {p.Name} | Licence Holder: {p.ManufacturerName}");

                return JsonConvert.SerializeObject(output.ToArray());
            }
        }

        private readonly IInstallationRequestRepository _installationRequestRepository;
        private readonly IMcsInstallationDataRepository _mcsInstallationDataRepository;
        private readonly ICreditLedgerService _creditLedgerService;

        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="installationRequestRepository">Installation request repo</param>
        /// <param name="mcsInstallationDataRepository">Mcs Installation Repository</param>
        /// <param name="creditLedgerService"></param>
        public DownloadRequestDataFileQueryHandler(
            ILogger<BaseRequestHandler<DownloadRequestDataFileQuery, ActionResult<Stream>>> logger,
            IInstallationRequestRepository installationRequestRepository,
            IMcsInstallationDataRepository mcsInstallationDataRepository,
            ICreditLedgerService creditLedgerService) : base(logger)
        {
            _installationRequestRepository = installationRequestRepository;
            _mcsInstallationDataRepository = mcsInstallationDataRepository;
            _creditLedgerService = creditLedgerService;
        }

        /// <summary>
        /// Handle the query
        /// </summary>
        /// <param name="request">The installations request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Data as a CSV download</returns>
        public override async Task<ActionResult<Stream>> Handle(DownloadRequestDataFileQuery request, CancellationToken cancellationToken)
        {
            var requestItem = await _installationRequestRepository.Get(request.RequestId);
            if (requestItem == null)
                return CannotFindRequest(request.RequestId);
            var requestDate = requestItem.RequestDate;

            var installations = await _mcsInstallationDataRepository.GetAll(i => i.InstallationRequestId == request.RequestId);

            var startDate = DateOnly.FromDateTime(requestItem.StartDate);
            var endDate = DateOnly.FromDateTime(requestItem.EndDate);
            var creditsResponse = await _creditLedgerService.GetInstallationCredits(startDate, endDate);
            if (!creditsResponse.IsSuccessStatusCode || creditsResponse.Result == null)
                return FailedToGetInstallationCredits(startDate, endDate);
            var credits = creditsResponse.Result;

            installations.ToList().ForEach(x => x.Credits = credits.FirstOrDefault(y => y.HeatPumpInstallationId == x.MidId)?.Value ?? 0m);

            using var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<HeatPumpInstallationMap>();
                csv.WriteRecords(installations);
            }

            return Responses.File(memoryStream.ToArray(), "text/csv", $"MCS-Data-Import-{requestDate:yyyy-MM-dd}.csv");
        }
    }
}