using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Commands
{
    public class RollbackGenerateNextYearsSchemeCommand : IRequest<ActionResult>
    {
        public Guid SchemeYearId { get; private set; }

        public RollbackGenerateNextYearsSchemeCommand(Guid schemeYearId)
        {
            SchemeYearId = schemeYearId;
        }   
    }
}
