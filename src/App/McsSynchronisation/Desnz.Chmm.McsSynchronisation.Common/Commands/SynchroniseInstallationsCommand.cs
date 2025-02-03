using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Common.Commands
{

    public class SynchroniseInstallationsCommand : IRequest<ActionResult>
    {
    }
}
