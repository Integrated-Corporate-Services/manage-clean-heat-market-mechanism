using Desnz.Chmm.BoilerSales.Common.Dtos.Annual;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Queries.Annual;

public class GetAnnualBoilerSalesQuery : IRequest<ActionResult<AnnualBoilerSalesDto>>
{
    /// <summary>
    /// Init query
    /// claims on any specific manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to get data for</param>
    /// <param name="schemeYearId">Scheme year to retrieve</param>
    public GetAnnualBoilerSalesQuery(Guid organisationId, Guid schemeYearId)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
    }

    public Guid OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
}
