using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands
{
    public class EditLicenceHolderCommand : IRequest<ActionResult>
    {
        public Guid LicenceHolderId { get; private set; }
        public DateOnly EndDate { get; private set; }
        public Guid? OrganisationId { get; private set; }

        public EditLicenceHolderCommand(Guid licenceHolderId, DateOnly endDate, Guid? organisationId)
        {
            LicenceHolderId = licenceHolderId;
            EndDate = endDate;
            OrganisationId = organisationId;
        }
    }
}
