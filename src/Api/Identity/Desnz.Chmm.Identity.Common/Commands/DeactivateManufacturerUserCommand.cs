using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands
{
    /// <summary>
    /// Command to deactivate an invited user
    /// </summary>
    public class DeactivateManufacturerUserCommand : IRequest<ActionResult>
    {
        public DeactivateManufacturerUserCommand(Guid userId, Guid organisationId)
        {
            UserId = userId;
            OrganisationId = organisationId;
        }
        public Guid UserId { get; set; }
        public Guid OrganisationId { get; set; }
    }
}
