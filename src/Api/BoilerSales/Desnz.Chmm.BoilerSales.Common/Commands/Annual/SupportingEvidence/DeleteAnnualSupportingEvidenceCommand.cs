using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;

public class DeleteAnnualSupportingEvidenceCommand : IRequest<ActionResult>
{
    /// <summary>
    /// Delete annual supporting evidence file from the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to delete data for</param>
    /// <param name="schemeYearId">Scheme year to delete for</param>
    /// <param name="fileName">File name to delete</param>
    /// <param name="isEditing">Is user editing existing boiler sales</param>
    public DeleteAnnualSupportingEvidenceCommand(Guid organisationId, Guid schemeYearId, string fileName, bool? isEditing = null)
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
