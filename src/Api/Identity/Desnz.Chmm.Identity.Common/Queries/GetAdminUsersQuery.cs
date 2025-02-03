using Desnz.Chmm.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries;

public class GetAdminUsersQuery : IRequest<ActionResult<List<ChmmUserDto>>>
{
}
