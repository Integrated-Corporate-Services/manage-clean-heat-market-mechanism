using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationRegisteredOfficeAddressCommand : IRequest<ActionResult>
    {
        public required Guid OrganisationId { get; init; }
        public required string LineOne { get; init; }
        public string? LineTwo { get; init; }
        public required string City { get; init; }
        public string? County { get; init; }
        public required string Postcode { get; init; }
    }
}
