using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationSchemeParticipationCommand : IRequest<ActionResult>
    {
        public required Guid OrganisationId { get; init; }
        public bool IsNonSchemeParticipant { get; init; }
    }
}
