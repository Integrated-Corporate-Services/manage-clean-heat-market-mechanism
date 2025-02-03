using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands
{
    /// <summary>
    /// Command to deactivate an invited user
    /// </summary>
    public class DeactivateAdminUserCommand : IRequest<ActionResult>
    {
        public Guid Id { get; set; }
    }
}
