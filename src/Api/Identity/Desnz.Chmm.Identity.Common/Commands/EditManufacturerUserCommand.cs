using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands
{
    public class EditManufacturerUserCommand : IRequest<ActionResult>
    {
        public Guid? OrganisationId { get; set; }
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? JobTitle { get; set; }
        public string? TelephoneNumber { get; set; }
    }
}
