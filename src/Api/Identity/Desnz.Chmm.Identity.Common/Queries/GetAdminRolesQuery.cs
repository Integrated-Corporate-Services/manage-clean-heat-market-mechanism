using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class GetAdminRolesQuery : IRequest<ActionResult<List<RoleDto>>>
    {
    }
}
