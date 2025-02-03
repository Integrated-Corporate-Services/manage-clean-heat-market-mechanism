using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands;

/// <summary>
/// Command to link a licence holder to an organisation
/// </summary>
public class LinkLicenceHolderCommand : IRequest<ActionResult>
{
    public Guid LicenceHolderId { get; private set; }

    public Guid OrganisationId { get; private set; }

    public DateOnly? StartDate { get; private set; }

    public LinkLicenceHolderCommand(Guid licenceHolderId, Guid organisationId, DateOnly? startDate = null)
    {
        LicenceHolderId = licenceHolderId;
        OrganisationId = organisationId;
        StartDate = startDate;
    }
}
