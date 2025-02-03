using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Commands.Quarterly.SupportingEvidence;

public class UploadQuarterlySupportingEvidenceCommand : IRequest<ActionResult>
{
    /// <summary>
    /// Upload annual supporting evidence files to the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to upload data for</param>
    /// <param name="schemeYearId">Scheme year to upload for</param>
    /// <param name="schemeYearQuarterId">Scheme year quarter to upload for</param>
    /// <param name="supportingEvidence">Files to upload</param>
    /// <param name="isEditing">Is editing</param>
    public UploadQuarterlySupportingEvidenceCommand(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId, List<IFormFile> supportingEvidence, bool isEditing = false)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        SchemeYearQuarterId = schemeYearQuarterId;
        SupportingEvidence = supportingEvidence;
        IsEditing = isEditing;
    }

    public Guid OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
    public Guid SchemeYearQuarterId { get; private set; }
    public List<IFormFile> SupportingEvidence { get; private set; }
    public bool IsEditing { get; private set; }
}
