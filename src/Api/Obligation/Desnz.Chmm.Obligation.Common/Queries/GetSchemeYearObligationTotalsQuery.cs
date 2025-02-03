using Desnz.Chmm.Obligation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Common.Queries
{
    /// <summary>
    /// Query the obligations belonging to a given organisation
    /// </summary>
    public class GetSchemeYearObligationTotalsQuery : IRequest<ActionResult<List<ObligationTotalDto>>>
    {
        /// <summary>
        /// Request the obligation summary totals of a given year
        /// </summary>
        /// <param name="schemeYearId">The scheme year to query</param>
        public GetSchemeYearObligationTotalsQuery(Guid schemeYearId)
        {
            SchemeYearId = schemeYearId;
        }

        public Guid SchemeYearId { get; private set; }
    }
}
