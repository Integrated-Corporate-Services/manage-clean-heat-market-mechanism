using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationDetailsCommand : IRequest<ActionResult>
    {
        public required Guid OrganisationId { get; init; }
        public required string Name { get; init; }
        public string? CompaniesHouseNumber { get; init; }
    }
}
