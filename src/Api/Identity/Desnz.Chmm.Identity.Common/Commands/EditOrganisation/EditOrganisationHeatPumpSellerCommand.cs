using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationHeatPumpSellerCommand : IRequest<ActionResult>
    {
        public required Guid OrganisationId { get; init; }
        public string[]? HeatPumpBrands { get; init; }
    }
}
