using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Common.Commands.Annual.VerificationStatement;

public class UploadAnnualVerificationStatementCommand : IRequest<ActionResult>
{
    /// <summary>
    /// Upload annual verification statement files to the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to upload data for</param>
    /// <param name="schemeYearId">Scheme year to upload for</param>
    /// <param name="verificationStatement">Files to upload</param>
    /// <param name="isEditing">Is user editing existing boiler sales</param>
    public UploadAnnualVerificationStatementCommand(Guid organisationId, Guid schemeYearId, List<IFormFile> verificationStatement, bool? isEditing = null)
    {
        ManufacturerId = organisationId;
        SchemeYearId = schemeYearId;
        VerificationStatement = verificationStatement;
        IsEditing = isEditing ?? false;
    }

    public Guid ManufacturerId { get; private set; }
    public Guid SchemeYearId { get; private set; }
    public List<IFormFile> VerificationStatement { get; private set; }
    public bool IsEditing { get; private set; }
}
