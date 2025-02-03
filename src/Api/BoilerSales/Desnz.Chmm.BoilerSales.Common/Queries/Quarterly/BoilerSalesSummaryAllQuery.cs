using Desnz.Chmm.BoilerSales.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Queries.Quarterly
{
    /// <summary>
    /// Query the all organisations' boiler sales for the given year
    /// </summary>
    public class BoilerSalesSummaryAllQuery : IRequest<ActionResult<List<BoilerSalesSummaryDto>>>
    {
        public BoilerSalesSummaryAllQuery(Guid schemeYearId)
        {
            SchemeYearId = schemeYearId;
        }

        public Guid SchemeYearId { get; }
    }
}
