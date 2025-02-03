using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands;

public class InviteAdminUserCommand : IRequest<ActionResult<Guid>>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Guid> RoleIds { get; set; }
}