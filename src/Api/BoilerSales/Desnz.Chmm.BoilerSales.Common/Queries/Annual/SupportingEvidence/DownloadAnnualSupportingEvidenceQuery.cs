using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Queries.Annual.SupportingEvidence;

public class DownloadAnnualSupportingEvidenceQuery : IRequest<ActionResult<Stream>>
{
    /// <summary>
    /// Download an annual supporting evidence file from the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to get data for</param>
    /// <param name="schemeYearId">Scheme year to retrieve</param>
    /// <param name="fileName">File name to get</param>
    /// <param name="isEditing">Is user editing existing boiler sales</param>
    public DownloadAnnualSupportingEvidenceQuery(Guid organisationId, Guid schemeYearId, string fileName, bool? isEditing = null)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        FileName = fileName;
        IsEditing = isEditing ?? false;
    }

    public Guid OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
    public string FileName { get; private set; }
    public bool IsEditing { get; private set; }
}
