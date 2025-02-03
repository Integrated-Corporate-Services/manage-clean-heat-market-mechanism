using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Queries.Annual.VerificationStatement;

public class GetAnnualVerificationStatementFileNamesQuery : IRequest<ActionResult<List<string>>>
{
    /// <summary>
    /// Get annual verification file names for the given manufacturer.
    /// </summary>
    /// <param name="organisationId">Manufacturer to get data for</param>
    /// <param name="schemeYearId">Scheme year to retrieve</param>
    /// <param name="isEditing">Is user editing existing boiler sales</param>
    public GetAnnualVerificationStatementFileNamesQuery(Guid organisationId, Guid schemeYearId, bool? isEditing = null)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        IsEditing = isEditing ?? false;
    }

    public Guid OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
    public bool IsEditing { get; private set; }
}
