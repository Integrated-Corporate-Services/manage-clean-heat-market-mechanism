using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Queries.Quarterly.SupportingEvidence;

public class GetQuarterlySupportingEvidenceFileNamesQuery : IRequest<ActionResult<List<string>>>
{
    /// <summary>
    /// Get the annual supporting evidence file names for the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to get data for</param>
    /// <param name="schemeYearId">Scheme year to retrieve</param>
    /// <param name="schemeYearQuarterId">Scheme year quarter to retrieve</param>
    /// <param name="isEditing">Is editing</param>
    public GetQuarterlySupportingEvidenceFileNamesQuery(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId, bool isEditing = false)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        SchemeYearQuarterId = schemeYearQuarterId;
        IsEditing = isEditing;
    }

    public Guid OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
    public Guid SchemeYearQuarterId { get; private set; }
    public bool IsEditing { get; private set; }
}
