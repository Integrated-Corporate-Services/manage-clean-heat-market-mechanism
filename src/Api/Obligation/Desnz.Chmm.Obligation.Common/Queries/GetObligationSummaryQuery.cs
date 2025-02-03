using Desnz.Chmm.Obligation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Common.Queries
{
    /// <summary>
    /// Query the obligations belonging to a given organisation
    /// </summary>
    public class GetObligationSummaryQuery : IRequest<ActionResult<ObligationSummaryDto>>
    {
        /// <summary>
        /// Request the obligation summary of a given organisation for the given year
        /// </summary>
        /// <param name="organisationId">The organisation to query</param>
        /// <param name="schemeYearId">The scheme year to query</param>
        public GetObligationSummaryQuery(Guid organisationId, Guid schemeYearId)
        {
            OrganisationId = organisationId;
            SchemeYearId = schemeYearId;
        }

        public Guid OrganisationId { get; private set; }
        public Guid SchemeYearId { get; private set; }
    }
}
