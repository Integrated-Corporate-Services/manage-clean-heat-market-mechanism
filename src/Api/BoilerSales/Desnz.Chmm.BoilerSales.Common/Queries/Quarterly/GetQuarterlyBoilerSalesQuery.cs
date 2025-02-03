using Desnz.Chmm.BoilerSales.Common.Dtos.Quarterly;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;

public class GetQuarterlyBoilerSalesQuery : IRequest<ActionResult<List<QuarterlyBoilerSalesDto>>>
{
    /// <summary>
    /// Init query
    /// </summary>
    /// <param name="organisationId">Manufacturer to get data for</param>
    /// <param name="schemeYearId">Scheme year to retrieve</param>
    public GetQuarterlyBoilerSalesQuery(Guid organisationId, Guid schemeYearId)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
    }

    public Guid? OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
}
