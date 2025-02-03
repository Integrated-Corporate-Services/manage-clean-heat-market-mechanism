using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Commands
{
    public class GenerateNextYearsSchemeaCommand : IRequest<ActionResult>
    {
        public Guid PreviousSchemeYearId { get; private set; }

        public GenerateNextYearsSchemeaCommand(Guid previousSchemeYearId)
        {
            PreviousSchemeYearId = previousSchemeYearId;
        }   
    }
}
