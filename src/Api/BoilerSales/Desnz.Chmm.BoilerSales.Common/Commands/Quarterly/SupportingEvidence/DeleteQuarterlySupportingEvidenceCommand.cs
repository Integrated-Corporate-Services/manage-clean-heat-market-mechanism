using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Commands.Quarterly.SupportingEvidence;

public class DeleteQuarterlySupportingEvidenceCommand : IRequest<ActionResult>
{
    /// <summary>
    /// Delete annual supporting evidence file from the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to delete data for</param>
    /// <param name="schemeYearId">Scheme year to delete for</param>
    /// <param name="schemeYearQuarterId">Scheme year quarter to delete for</param>
    /// <param name="fileName">File name to delete</param>
    /// <param name="isEditing">Is editing</param>
    public DeleteQuarterlySupportingEvidenceCommand(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId, string fileName, bool isEditing = false)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        SchemeYearQuarterId = schemeYearQuarterId;
        FileName = fileName;
        IsEditing = isEditing;
    }

    public Guid OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
    public Guid SchemeYearQuarterId { get;private set; }
    public string FileName { get; private set; }
    public bool IsEditing { get; private set; }
}
