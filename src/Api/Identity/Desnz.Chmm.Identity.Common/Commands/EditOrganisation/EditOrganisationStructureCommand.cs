using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationStructureCommand : IRequest<ActionResult>
    {
        public required Guid OrganisationId { get; init; }
        public required bool IsOnBehalfOfGroup { get; init; }
    }
}
