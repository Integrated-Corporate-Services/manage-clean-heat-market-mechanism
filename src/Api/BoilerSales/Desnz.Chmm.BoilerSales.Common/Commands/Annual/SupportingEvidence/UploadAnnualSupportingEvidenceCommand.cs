using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;

public class UploadAnnualSupportingEvidenceCommand : IRequest<ActionResult>
{
    /// <summary>
    /// Upload annual supporting evidence files to the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to upload data for</param>
    /// <param name="schemeYearId">Scheme year to upload for</param>
    /// <param name="supportingEvidence">Files to upload</param>
    /// <param name="isEditing">Is user editing existing boiler sales</param>
    public UploadAnnualSupportingEvidenceCommand(Guid organisationId, Guid schemeYearId, List<IFormFile> supportingEvidence, bool? isEditing = null)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        SupportingEvidence = supportingEvidence;
        IsEditing = isEditing ?? false;
    }

    public Guid OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
    public List<IFormFile> SupportingEvidence { get; private set; }
    public bool IsEditing { get; private set; }
}
