using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationSeniorResponsibleOfficerCommand : IRequest<ActionResult>
    {
        public required Guid OrganisationId { get; init; }
        public string? Name { get; init; }
        public string? JobTitle { get; init; }
        public string? TelephoneNumber { get; init; }
        public required bool IsApplicantSeniorResponsibleOfficer { get; init; }
    }
}
