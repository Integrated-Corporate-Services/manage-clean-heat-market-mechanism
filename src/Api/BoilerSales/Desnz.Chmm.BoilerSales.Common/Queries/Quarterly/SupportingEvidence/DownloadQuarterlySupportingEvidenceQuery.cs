using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Queries.Quarterly.SupportingEvidence;

public class DownloadQuarterlySupportingEvidenceQuery : IRequest<ActionResult<Stream>>
{
    /// <summary>
    /// Download an annual supporting evidence file from the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to get data for</param>
    /// <param name="schemeYearId">Scheme year to retrieve</param>
    /// <param name="schemeYearQuarterId">Scheme year quarter to retrieve</param>
    /// <param name="fileName">File name to get</param>
    public DownloadQuarterlySupportingEvidenceQuery(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId, string fileName)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        SchemeYearQuarterId = schemeYearQuarterId;
        FileName = fileName;
    }

    public Guid OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
    public Guid SchemeYearQuarterId { get; private set; }
    public string FileName { get; private set; }
}
