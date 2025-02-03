using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands;

/// <summary>
/// Command to unlink a licence holder from an organisation
/// </summary>
public class EndLinkLicenceHolderCommand : IRequest<ActionResult>
{
    public Guid LicenceHolderId { get; private set; }
    public Guid OrganisationId { get; private set; }
    public DateOnly EndDate { get; private set; }
    public Guid? OrganisationIdToTransfer { get; private set; }

    public EndLinkLicenceHolderCommand(Guid licenceHolderId, Guid organisationId, DateOnly endDate, Guid? organisationIdToTransfer = null)
    {
        LicenceHolderId = licenceHolderId;
        OrganisationId = organisationId;
        EndDate = endDate;
        OrganisationIdToTransfer = organisationIdToTransfer;
    }
}
