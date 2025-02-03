using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands
{
    public class EditAdminUserCommand : IRequest<ActionResult>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> RoleIds { get; set; }
    }
}
