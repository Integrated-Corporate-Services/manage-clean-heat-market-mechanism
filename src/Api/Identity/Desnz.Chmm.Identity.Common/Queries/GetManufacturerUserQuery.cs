using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries;

public class GetManufacturerUserQuery : IRequest<ActionResult<ViewManufacturerUserDto>>
{
    public Guid OrganisationId { get; private set; }
    public Guid UserId { get; private set; }

    public GetManufacturerUserQuery(Guid organisationId, Guid userId)
    {
        OrganisationId = organisationId;
        UserId = userId;
    }

}
