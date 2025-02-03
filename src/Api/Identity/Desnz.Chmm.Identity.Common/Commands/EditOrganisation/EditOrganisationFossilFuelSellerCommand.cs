using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationFossilFuelSellerCommand : IRequest<ActionResult>
    {
        public required Guid OrganisationId { get; init; }
        public required bool IsFossilFuelBoilerSeller { get; init; }
    }
}
