using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries;

public class GetManufacturerUsersQuery : IRequest<ActionResult<List<ViewManufacturerUserDto>>>
{
    public Guid OrganisationId { get; set; }
}
