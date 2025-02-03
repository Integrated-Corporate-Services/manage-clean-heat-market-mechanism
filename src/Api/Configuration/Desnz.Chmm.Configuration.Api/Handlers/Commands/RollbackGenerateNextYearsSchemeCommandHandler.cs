using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Api.Handlers.Commands
{
    public class RollbackGenerateNextYearsSchemeCommandHandler : BaseRequestHandler<RollbackGenerateNextYearsSchemeCommand, ActionResult>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;

        public RollbackGenerateNextYearsSchemeCommandHandler(
            ILogger<BaseRequestHandler<RollbackGenerateNextYearsSchemeCommand, ActionResult>> logger,
            ISchemeYearRepository schemeYearRepository) : base(logger)
        {
            _schemeYearRepository = schemeYearRepository;
        }

        public override async Task<ActionResult> Handle(RollbackGenerateNextYearsSchemeCommand command, CancellationToken cancellationToken)
        {
            var schemeYear = await _schemeYearRepository.GetSchemeYear(i => i.PreviousSchemeYearId == command.SchemeYearId, true, true, true);
            if (schemeYear != null)
            {
                await _schemeYearRepository.RemoveCascade(schemeYear);
            }

            return Responses.Ok();
        }
    }
}
