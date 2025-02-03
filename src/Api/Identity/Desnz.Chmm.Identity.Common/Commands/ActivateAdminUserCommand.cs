using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands
{

    /// <summary>
    /// Command to activate an invited user
    /// </summary>
    public class ActivateAdminUserCommand : IRequest<ActionResult>
    {
        public Guid Id { get; set; }
    }
}
