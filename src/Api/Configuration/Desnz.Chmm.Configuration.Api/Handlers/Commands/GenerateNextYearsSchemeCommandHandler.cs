using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Api.Handlers.Commands
{
    public class GenerateNextYearsSchemeCommandHandler : BaseRequestHandler<GenerateNextYearsSchemeaCommand, ActionResult>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;

        public GenerateNextYearsSchemeCommandHandler(
            ILogger<BaseRequestHandler<GenerateNextYearsSchemeaCommand, ActionResult>> logger,
            ISchemeYearRepository schemeYearRepository) : base(logger)
        {
            _schemeYearRepository = schemeYearRepository;
        }

        public override async Task<ActionResult> Handle(GenerateNextYearsSchemeaCommand command, CancellationToken cancellationToken)
        {
            var schemeYear = await _schemeYearRepository.GetSchemeYear(i => i.PreviousSchemeYearId == command.PreviousSchemeYearId, true, true);
            if (schemeYear == null)
                return CannotLoadSchemeYear(command.PreviousSchemeYearId);

            var nextSchemeYear = await _schemeYearRepository.GetSchemeYear(i => i.PreviousSchemeYearId == schemeYear.Id);
            if (nextSchemeYear != null)
                return SchemeYearExists(command.PreviousSchemeYearId);

            var values = schemeYear.GenerateFuelTypeWeightingValues();

            var weightingValues = await _schemeYearRepository.CreateFuelTypeWeightingValues(values);

            nextSchemeYear = schemeYear.GenerateNext(values);

            var id = await _schemeYearRepository.Create(nextSchemeYear);

            return Responses.Created(id);
        }
    }
}
