using Desnz.Chmm.BoilerSales.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Queries.Quarterly
{
    /// <summary>
    /// Query the organisation's boiler sales for the given year
    /// </summary>
    public class BoilerSalesSummaryQuery : IRequest<ActionResult<BoilerSalesSummaryDto>>
    {
        public BoilerSalesSummaryQuery(Guid organisationId, Guid schemeYearId)
        {
            OrganisationId = organisationId;
            SchemeYearId = schemeYearId;
        }

        public Guid OrganisationId { get; }
        public Guid SchemeYearId { get; }
    }
}
