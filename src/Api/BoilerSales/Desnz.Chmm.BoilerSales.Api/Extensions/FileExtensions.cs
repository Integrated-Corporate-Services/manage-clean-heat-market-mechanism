
namespace Desnz.Chmm.BoilerSales.Api.Extensions
{
    public static class FileExtensions
    {
        public static string BuildObjectKeyForAnnualBoilerSales(this IFormFile file, Guid organisationId, Guid schemeYearId)
            => file.FileName.BuildObjectKeyForAnnualBoilerSales(organisationId, schemeYearId);

        public static string BuildObjectKeyForAnnualBoilerSales(this string fileName, Guid organisationId, Guid schemeYearId)
            => $"{organisationId}/{schemeYearId}/{fileName}";

        public static string BuildObjectKeyForQuarterlyBoilerSales(this IFormFile file, Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId)
            => file.FileName.BuildObjectKeyForQuarterlyBoilerSales(organisationId, schemeYearId, schemeYearQuarterId);

        public static string BuildObjectKeyForQuarterlyBoilerSales(this string fileName, Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId)
            => $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}/{fileName}";

    }
}
