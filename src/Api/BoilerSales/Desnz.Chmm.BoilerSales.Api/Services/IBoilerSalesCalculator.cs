using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.Configuration.Common.Dtos;

namespace Desnz.Chmm.BoilerSales.Api.Services
{
    public interface IBoilerSalesCalculator
    {
        /// <summary>
        /// Generate the boiler sales summary for the given organiastion based on the provided sales figures
        /// </summary>
        /// <param name="organisationId">The organisation Id</param>
        /// <param name="annualSales">The annual sales figures for the organisation</param>
        /// <param name="quarterlySales">The quarterly sales figures for the organisation, can be excluded if annual sales are provided</param>
        /// <param name="obligationCalculations">The obligation calculations</param>
        /// <returns></returns>
        BoilerSalesSummaryDto GenerateBoilerSalesSummary(Guid organisationId, AnnualBoilerSales? annualSales, List<QuarterlyBoilerSales>? quarterlySales, ObligationCalculationsDto obligationCalculations);
    }
}
