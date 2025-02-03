using Desnz.Chmm.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries;

public class GetAdminUserQuery : IRequest<ActionResult<ChmmUserDto>>
{
    public Guid UserId { get; set; }
    public object OrganisationId { get; set; }
}
