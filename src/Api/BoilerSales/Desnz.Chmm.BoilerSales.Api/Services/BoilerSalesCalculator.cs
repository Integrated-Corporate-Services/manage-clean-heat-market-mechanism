using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.Configuration.Common.Dtos;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.Api.Services
{
    public class BoilerSalesCalculator : IBoilerSalesCalculator
    {
        /// <summary>
        /// Generate the boiler sales summary for the given organiastion based on the provided sales figures
        /// </summary>
        /// <param name="organisationId">The organisation Id</param>
        /// <param name="annualSales">The annual sales figures for the organisation</param>
        /// <param name="quarterlySales">The quarterly sales figures for the organisation, can be excluded if annual sales are provided</param>
        /// <param name="obligationCalculations">The obligation calculations</param>
        /// <returns></returns>
        public BoilerSalesSummaryDto GenerateBoilerSalesSummary(
            Guid organisationId, 
            AnnualBoilerSales? annualSales, 
            List<QuarterlyBoilerSales>? quarterlySales, 
            ObligationCalculationsDto obligationCalculations)
        {
            if (annualSales != null)
            {
                var annualStatus = BoilerSalesSummarySubmissionStatus.AnnualSubmitted;
                if (annualSales.Status == BoilerSalesStatus.Approved)
                    annualStatus = BoilerSalesSummarySubmissionStatus.AnnualApproved;

                return new BoilerSalesSummaryDto(
                    organisationId,
                    annualSales.Gas,
                    annualSales.Oil,
                    Math.Max(annualSales.Gas - obligationCalculations.GasBoilerSalesThreshold, 0),
                    Math.Max(annualSales.Oil - obligationCalculations.OilBoilerSalesThreshold, 0),
                    annualStatus
                );
            }

            var quarterGas = quarterlySales.Sum(s => s.Gas);
            var quarterOil = quarterlySales.Sum(s => s.Oil);

            var status = BoilerSalesSummarySubmissionStatus.NotSubmitted;
            if (quarterlySales.Any())
                status = BoilerSalesSummarySubmissionStatus.QuarterSubmitted;

            // TODO: Generate in a calculator!
            return new BoilerSalesSummaryDto(
                organisationId,
                quarterGas,
                quarterOil,
                Math.Max(quarterGas - obligationCalculations.GasBoilerSalesThreshold, 0),
                Math.Max(quarterOil - obligationCalculations.OilBoilerSalesThreshold, 0),
                status
            );
        }
    }
}
