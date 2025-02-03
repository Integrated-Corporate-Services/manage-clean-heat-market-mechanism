using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationCreditContactDetailsCommand : IRequest<ActionResult>
    {
        public required Guid OrganisationId { get; init; }
        public string? Name { get; init; }
        public string? Email { get; init; }
        public string? TelephoneNumber { get; init; }
    }
}
