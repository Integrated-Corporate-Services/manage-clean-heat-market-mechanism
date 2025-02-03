using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Commands;

public class CarryOverNewLicenceHoldersCommand : IRequest<ActionResult>
{
    public CarryOverNewLicenceHoldersCommand(Guid organisationId, Guid licenceHolderId, Guid previousSchemeYearId, DateOnly startDate)
    {
        OrganisationId = organisationId;
        LicenceHolderId = licenceHolderId;
        PreviousSchemeYearId = previousSchemeYearId;
        StartDate = startDate;
    }

    public Guid OrganisationId { get; private set; }
    public Guid LicenceHolderId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public Guid PreviousSchemeYearId { get; private set; }
}
