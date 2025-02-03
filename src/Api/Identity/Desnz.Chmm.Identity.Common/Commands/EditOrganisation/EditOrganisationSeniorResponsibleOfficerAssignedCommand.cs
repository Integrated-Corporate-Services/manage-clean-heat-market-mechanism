using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationSeniorResponsibleOfficerAssignedCommand : IRequest<ActionResult>
    {
        public required Guid OrganisationId { get; init; }
        public required Guid UserId { get; init; }
    }
}
