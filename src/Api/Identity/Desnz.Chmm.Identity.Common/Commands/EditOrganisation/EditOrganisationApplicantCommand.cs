using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationApplicantCommand : IRequest<ActionResult>
    {
        public required Guid OrganisationId { get; init; }
        public required string Name { get; init; }
        public required string JobTitle { get; init; }
        public required string TelephoneNumber { get; init; }
    }
}
