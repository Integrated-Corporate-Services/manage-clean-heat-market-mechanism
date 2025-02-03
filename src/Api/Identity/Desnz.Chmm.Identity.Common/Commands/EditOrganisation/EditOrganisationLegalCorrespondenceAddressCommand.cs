using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationLegalCorrespondenceAddressCommand : IRequest<ActionResult>
    {
        public required Guid OrganisationId { get; init; }
        public string? LineOne { get; set; }
        public string? LineTwo { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? Postcode { get; set; }
        public required string LegalAddressType { get; init; }
    }
}
